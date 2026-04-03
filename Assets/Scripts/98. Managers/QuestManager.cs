using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>, IDataStart
{
    [Header("Config/References")]
    public QuestPoolSO pool;
    public int playerStage = 0; // 플레이어 진행 스테이지

    private ICurrencyService currency; // 재화
    private IInventoryService inventory; // 인벤

    private QuestSaveData saveData; // 저장데이터
    private Dictionary<int, QuestDataSO> questDataDict; // 퀘스트 데이터 Dict
    public IReadOnlyList<QuestData> QuestDataList => questDataList;
    private List<QuestData> questDataList;

    private readonly TimeSpan kstOffset = TimeSpan.FromHours(9); // KST UTC+9

    private Guid onComboEK, onScoreEK, onBlockEK, onItemEK;

    public UniTask DataStartAsync(CancellationToken cancelToken)
    {
        questDataDict = pool.questList.ToDictionary(x => x.id, x => x); // 퀘스트 데이터 Dict 초기화
        questDataList = new(); // 퀘스트 런타임데이터 리스트 초기화

        LoadOrInitDaily(); // 일일퀘스트 로드 및 초기화

        RegistEvent(); // 이벤트 초기화

        return UniTask.CompletedTask;
    }

    #region 이벤트

    private void RegistEvent()
    {
        onComboEK = EventManager.Instance.Subscribe<QuestEventType, int>(QuestEventType.ComboChanged, OnComboChanged);
        onScoreEK = EventManager.Instance.Subscribe<QuestEventType, int>(QuestEventType.ScoreChanged, OnScoreChanged);
        onBlockEK =  EventManager.Instance.Subscribe<QuestEventType, BlockType>(QuestEventType.BlockDestory, OnBlockDestory);
        onItemEK = EventManager.Instance.Subscribe<QuestEventType, ShopItemType>(QuestEventType.ItemUse, OnItemUse);
    }

    private void RemoveEvent()
    {
        EventManager.Instance.Unsubscribe(QuestEventType.ComboChanged, onComboEK);
        EventManager.Instance.Unsubscribe(QuestEventType.ScoreChanged, onScoreEK);
        EventManager.Instance.Unsubscribe(QuestEventType.BlockDestory, onBlockEK);
        EventManager.Instance.Unsubscribe(QuestEventType.ItemUse, onItemEK);
    }

    #endregion

    /// <summary>
    /// 일일퀘스트 로드 및 초기화
    /// </summary>
    private void LoadOrInitDaily()
    {
        saveData = SaveManager.Instance.QuestSaveManager.QuestSaveData; // 저장된 퀘스트 데이터를 가져온다
        
        // 저장된 데이터가 없거나 리셋을 해야한다(하루가 지났다)면 데이터 초기화
        if (saveData == null || CheckReset())
        {
            RollDailyQuests();
            Save();
        }

        foreach(var progress in saveData.progressList)
        {
            if(questDataDict.TryGetValue(progress.questID, out var so))
            {
                QuestData data = new QuestData(so);
                data.LoadState(progress.conditionStateJson);
                questDataList.Add(data);
            }
        }
    }

    /// <summary>
    /// 데이터 리셋여부 체크
    /// </summary>
    /// <returns></returns>
    private bool CheckReset()
    {
        // 저장된 데이터가 없다면 즉시 true리턴
        if (saveData == null)
            return true;

        return DateTime.UtcNow.Ticks >= saveData.nextResetUtcTicks; // UTC기준 현재 시간이 저장된 시간보다 더 크다면 true 킵
    }

    /// <summary>
    /// 퀘스트 데이터 초기화 및 갱신
    /// </summary>
    private void RollDailyQuests()
    {
        DateTime currentUtc = DateTime.UtcNow; // 현재 utc
        DateTime currentKst = currentUtc + kstOffset; // 현재 kst
        DateTime nextKstMidnight = currentKst.Date.AddDays(1); // 현재 kst기준 하루 이후의 날짜의 자정시간을 구한다.
        DateTime nextResetUtc = nextKstMidnight - kstOffset; // kst(9)기준만큼 빼서 Utc기준 초기화 날짜를 구한다.

        // 퀘스트 중, 현재 가능한 퀘스트들을 가져온다.
        var availableQuestList = pool.questList
                                .Where(quest => quest != null && playerStage >= quest.minStage)
                                .ToList();

        
        var recentQuest = new HashSet<int>(saveData?.recentQuestIDList ?? Array.Empty<int>().ToList()); // 최근에 나왔던 퀘스트 ID를 확인해서 재등장하지 못하게 함 킵

        var picked = WeightedPickQuest(availableQuestList, pool.dailyQuestCount, recentQuest); // 퀘스트를 선별한다.

        // 뽑힌 퀘스트가 일일 퀘스트의 수보다 낮을 때 필터(최근 사용한 퀘스트)를 제외하고 한번 더 선별한다.
        if(picked.Count < pool.dailyQuestCount && availableQuestList.Count > 0)
        {
            picked = WeightedPickQuest(availableQuestList, pool.dailyQuestCount, null);
        }

        // 세이브 데이터 교체
        saveData = new QuestSaveData
        {
            nextResetUtcTicks = nextResetUtc.Ticks,
            activeQuestIDList = picked.Select(quest => quest.id).ToList(),
            progressList = picked.Select(quest => new QuestProgress
            {
                questID = quest.id,
                progress = 0,
                completed = false,
                claimed = false,
                conditionStateJson = "",
            }).ToList(),

            recentQuestIDList = MergeRecentQuest(saveData?.recentQuestIDList ?? Array.Empty<int>().ToList(), picked.Select(quest => quest.id).ToList(), pool.cooldownDays),
            lastAssignedKstDate = currentKst.ToString("yyyy-MM-dd")
        };
    }

    /// <summary>
    /// 가중치 기준으로 퀘스트들을 뽑는다.
    /// </summary>
    /// <param name="questList"></param>
    /// <param name="dailyCount"></param>
    /// <param name="except"></param>
    private List<QuestDataSO> WeightedPickQuest(List<QuestDataSO> questList, int dailyCount, HashSet<int> except)
    {
        List<QuestDataSO> result = new List<QuestDataSO>();

        var candidate = except == null ? questList : questList.Where(quest => !except.Contains(quest.id)).ToList(); // 예외가 없으면 모든 퀘스트가 후보자이며, 예외가 존재하면 그 예외 퀘스트ID를 제외한 나머지 퀘스트가 후보자가 된다.

        // 후보자가 없다면 빈 리스트 그대로 리턴
        if(candidate.Count <= 0)
            return result;

        float rand = Random.Value;

        // 일일 퀘스트 개수 혹은 후보자 개수만큼 반복
        for(int i = 0; i < dailyCount && candidate.Count > 0; i++)
        {
            // 퀘스트의 가중치들을 전부 합한다. > 모든 가중치는 최소 1이상이어야 하므로 Max를 이용해서 아무리 낮아도 1이도록 만든다.
            // 가중치 추출 로직이 망가지지 않게 하려는 안전장치
            int totalWeight = candidate.Sum(quest => Mathf.Max(1, quest.weight));

            int pick = Random.Range(0, totalWeight); // 0과 가중치합 사이의 랜덤한 수를 뽑는다, 랜덤가중치

            int acc = 0; // 누적 가중치

            QuestDataSO pickedQuest = null;

            // 후보자들을 순회하면서 랜덤가중치가 누적가중치보다 낮을 때 해당 후보자를 뽑는다.
            // 이 부분이 퀘스트 선별에 대한 로직이다 > 지금은 간단하게 이런 식으로 작성
            foreach (var quest in candidate)
            {
                acc += Mathf.Max(1, quest.weight);
                if (pick < acc)
                {
                    pickedQuest = quest;
                    break;
                }
            }
            if(pickedQuest != null)
            {
                result.Add(pickedQuest); // 최종 결과에 퀘스트 추가
                candidate.Remove(pickedQuest); // 후보자에서 퀘스트 제거
            }
        }

        return result;
    }

    /// <summary>
    /// 이전(어제)에 받았던 퀘스트와, 현재(오늘) 받은 퀘스트를 병합시킨다.<br></br>
    /// cooldownDays만큼 지난 퀘스트를 제거한다.
    /// </summary>
    /// <param name="prevRecentQuestList"></param>
    /// <param name="currentQuestList"></param>
    /// <param name="cooldownDays"></param>
    /// <returns></returns>
    private List<int> MergeRecentQuest(List<int> prevRecentQuestList, List<int> currentQuestList, int cooldownDays)
    {
        List<int> result = new List<int>();

        // 대개 prev는 8(어제 + 그저께)이며 current는 4(오늘)이다.
        if (prevRecentQuestList != null)
            result.AddRange(prevRecentQuestList);
        if(currentQuestList != null)
            result.AddRange(currentQuestList);

        // cooldownDays(현재 2임) * currentQuestList.Count(보통 4임) 혹은 4를 하여, 최근 퀘스트의 개수를 제한한다.
        int max = Mathf.Max(0, cooldownDays) * Mathf.Max(1, currentQuestList?.Count ?? 4);

        // result에 존재하는 예전 퀘스트(cooldownDay만큼의 일자가 지난) 수 만큼의 퀘스트를 제외시킨다.
        if(max > 0 && result.Count > max)
        {
            result = result.Skip(result.Count - max).ToList();
        }

        return result.Distinct().ToList(); // 중복 제거
    }


    /// <summary>
    /// 퀘스트 내용 갱신<br></br>
    /// 퀘스트 내용 값이 int인 퀘스트 전용
    /// </summary>
    /// <param name="type"></param>
    /// <param name="mutate"></param>
    private void UpdateCondition(QuestConditionType type, Func<int, int> mutate)
    {
        bool isDirty = false;
        foreach (var questProgress in saveData.progressList)
        {
            QuestDataSO data = GetData(questProgress.questID); // 퀘스트 ID에 해당하는 퀘스트 데이터를 가져온다

            // 퀘스트가 없거나, conditionType과 맞지않거나, 이미 완료된 퀘스트라면 넘긴다. 킵
            if (data == null || data.conditionType != type || questProgress.completed)
                continue;

            int old = questProgress.progress; // 기존 퀘스트 진행도
            int update = mutate(old); // 새로운 퀘스트 진행도 확인

            // 진행도가 다르다면 갱신
            if(update != old)
            {
                questProgress.progress = update; // 데이터 갱신

                // 진행수치가 목표를 넘었다면 클리어로 전환
                if (questProgress.progress >= data.targetValue)
                    questProgress.completed = true;

                isDirty = true;
            }
        }

        // 변경되었으면 저장
        if (isDirty)
            Save();
    }

    private void UpdateCondition(QuestEvent questEvent)
    {
        bool isDirty = false;

        foreach(var questData in questDataList)
        {
            if (questData.baseData.conditionType != questEvent.type)
                continue;

            questData.OnEvent(questEvent);

            var progress = saveData.progressList.Find(progress => progress.questID == questData.id);

            // 변경된 값이 다르다면
            if(!progress.conditionStateJson.Equals(questData.SaveState()))
            {
                isDirty = true;
            }

            // 퀘스트가 끝났으면
            if (questData.IsCompleted())
            {
                progress.completed = true;
                isDirty = true;
            }
        }
        // 변경되었으면 저장
        if (isDirty)
            Save();
    }

    /// <summary>
    /// 보상 지급
    /// </summary>
    /// <param name="questID"></param>
    /// <returns></returns>
    public async UniTask<bool> PayReward(int questID)
    {
        var questProgress = saveData.progressList.FirstOrDefault(progress => progress.questID == questID); // questID의 진행상황을 가져온다.
        if(questProgress == null)
        {
            Debug.Log("퀘스트가 없음");
            return false;
        }

        // 완료하지 않았거나, 보상을 이미 받았다면 종료
        if (!questProgress.completed || questProgress.claimed)
            return false;

        // 퀘스트 데이터를 가져온다.
        var data = GetData(questID);
        if (data == null)
            return false;

        // 보상 지급
        if (data.reward.gold > 0)
            GameManager.Instance.AddGold(data.reward.gold);

        if (data.reward.dia > 0)
            GameManager.Instance.AddDia(data.reward.dia);

        if(data.reward.itemIds.Length > 0)
        {
            for(int i = 0; i < data.reward.itemIds.Length; i++)
            {
                await SaveManager.Instance.ItemSaveManager.UpdatePurchase(data.reward.itemIds[i], data.reward.itemQtys[i]);
            }
        }

        // 보상 지급 완료 처리
        questProgress.claimed = true;

        // 저장
        Save();
        return true;
    }

    /// <summary>
    /// 퀘스트 보상 모두 받기
    /// </summary>
    /// <returns></returns>
    public async UniTask<int> PayRewardAll()
    {
        int count = 0;
        foreach(var progress in saveData.progressList)
        {
            if(progress.completed && !progress.claimed)
            {
                if (await PayReward(progress.questID))
                    count++;
            }
        }
        return count;
    }

    public void Save()
    {
        foreach(var questData in questDataList)
        {
            var progress = saveData.progressList.Find(progress => progress.questID == questData.id);
            progress.conditionStateJson = questData.SaveState();
        }

        SaveManager.Instance.QuestSaveManager.UpdateData(saveData);
    }
    /// <summary>
    /// 모든 일일 퀘스트 진행도 확인
    /// </summary>
    public IReadOnlyList<QuestProgress> ActiveProgresses => saveData?.progressList ?? new List<QuestProgress>();

    /// <summary>
    /// questID에 해당하는 퀘스트 데이터를 가져온다
    /// </summary>
    /// <param name="questID"></param>
    /// <returns></returns>
    public QuestDataSO GetData(int questID)
    {
        if (questDataDict == null)
            return null;

        if(questDataDict.TryGetValue(questID, out var data))
        {
            return data;
        }

        return null;
    }

    /// <summary>
    /// UTC기준 초기화 시점을 얻는다.
    /// </summary>
    /// <returns></returns>
    public DateTime GetNextResetUtc() => new DateTime(saveData.nextResetUtcTicks, DateTimeKind.Utc);

    /// <summary>
    /// 콤보 변경 시
    /// </summary>
    /// <param name="combo"></param>
    public void OnComboChanged(int newCombo)
    {
        UpdateCondition(new QuestEvent() { type = QuestConditionType.ComboReach, newCombo = newCombo });
    }

    /// <summary>
    /// 점수 변경 시
    /// </summary>
    /// <param name="score"></param>
    public void OnScoreChanged(int newScore)
    {
        UpdateCondition(new QuestEvent() { type = QuestConditionType.ScoreReach, newScore = newScore });
    }

    /// <summary>
    /// 블록 파괴 시
    /// </summary>
    /// <param name="type"></param>
    public void OnBlockDestory(BlockType blockType)
    {
        // 현재 퀘스트 중 type에 맞는 블록파괴하는 퀘스트가 존재하면 그 퀘스트만 갱신
        UpdateCondition(new QuestEvent() { type = QuestConditionType.DestoryBlock, blockType = blockType });
    }

    /// <summary>
    /// 아이템 사용 시
    /// </summary>
    /// <param name="itemType"></param>
    public void OnItemUse(ShopItemType itemType)
    {
        // 현재 퀘스트 중 type에 맞는 아이템을 사용하는 퀘스트가 존재하면 그 퀘스트만 갱신

    }

    private void OnDestroy()
    {
        RemoveEvent();
    }
}
