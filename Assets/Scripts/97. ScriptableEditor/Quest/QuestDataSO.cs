using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct QuestEvent
{
    // 퀘스트 타입 > 반드시 설정해야한다.
    public QuestConditionType type;

    // BlockDestroyed 용
    public BlockType blockType;

    // ComboChanged 용
    public int newCombo;

    // ScoreReach 용
    public int newScore;

    // StageClear 용
    public bool success;
    public int finalScore;
    public int usedWeakCount;
}

[Serializable]
public struct QuestReward
{
    public int gold; // 골드 보상
    public int dia; // 다이아몬드 보상
    public int[] itemIds; // 아이템 보상
    public int[] itemQtys; // must match length of itemIds // 아이템 수량, itemIds과 index가 같은 부분의 수량이다.
}

[CreateAssetMenu(fileName = "QuestData", menuName = "KGC/Quest/QuestData")]
public class QuestDataSO : ScriptableObject
{
    [Header("퀘스트 데이터")]
    public int id;
    public string title;

    [TextArea]
    public string desc;

    [Header("퀘스트 상태")]
    public QuestConditionType conditionType;
    public int targetValue = 1; // 목표 수치

    public ConditionLogic conditionLogic = ConditionLogic.AND; // 추가 조건
    public ProgressDisplayMode displayMode = ProgressDisplayMode.Min;

    [Header("Selection & Gating")]
    public int weight = 1; // 가중치(중요도) 즉 우선순위
    public int minStage = 0; // 최소 스테이지
    public string[] tags; // 향후 필터를 위한 선택사항

    //public ScriptableObject[] conditionArr; // 퀘스트SO Array

    public QuestConditionDataSO[] conditionDataArr; // 퀘스트 조건 인터페이스 Array

    [Header("보상")]
    public QuestReward reward; // 퀘스트 보상

    //private void OnEnable()
    //{
    //    questDataArr = conditionArr.OfType<IQuestCondition>().ToArray();
    //}

    //public void OnStart()
    //{
    //    foreach(var quest in questDataArr)
    //    {
    //        quest.OnStart();
    //    }
    //}

    //public void OnEvent(QuestEvent questEvent)
    //{
    //    foreach (var quest in questDataArr)
    //    {
    //        quest.OnEvent(questEvent);
    //    }
    //}

    //public int GetProgress() => questDataArr.Sum(quest => quest.GetProgress());

    //public bool IsCompleted()
    //{
    //    return conditionLogic == ConditionLogic.AND ?
    //        questDataArr.All(condition => condition.IsCompleted(targetValue)) :
    //        questDataArr.Any(condition => condition.IsCompleted(targetValue));
    //}
}

public sealed class QuestData
{
    public int id;
    public readonly QuestDataSO baseData;
    public int ItemCount => conditionArr.Length;

    private readonly IQuestConditionRuntime[] conditionArr;

    public QuestData(QuestDataSO data)
    {
        baseData = data;

        id = data.id;

        if(data.conditionDataArr != null)
        {
            conditionArr = data.conditionDataArr
                                .Where(d => d != null)
                                .Select(d => d.CreateRuntime())
                                .ToArray();
        }
        else
        {
            conditionArr = Array.Empty<IQuestConditionRuntime>();
        }
    }

    public void OnStart()
    {
        foreach(var condition in conditionArr)
        {
            condition.OnStart();
        }
    }

    public void OnEvent(in QuestEvent e)
    {
        foreach (var condition in conditionArr)
        {
            condition.OnEvent(e);
        }
    }

    public string[] GetItemName()
    {
        return conditionArr.Select(data => data.GetItemName()).ToArray();
    }

    public int GetProgress(int index)
    {
        return conditionArr[index].GetProgress();
    }

    /// <summary>
    /// 퀘스트 조건의 진행도를 가져온다
    /// </summary>
    /// <returns></returns>
    public int GetAllProgress()
    {
        // 모든 조건들의 진행도를 가져온다.
        var progressArr = conditionArr.Select(cond => Mathf.Clamp(cond.GetProgress(), 0, baseData.targetValue)).ToArray();

        int finalValue = -1;
        switch(baseData.displayMode)
        {
            case ProgressDisplayMode.Min:
                finalValue = progressArr.Length > 0 ? progressArr.Min() : 0;
                break;
            case ProgressDisplayMode.Max:
                finalValue = progressArr.Length > 0 ? progressArr.Max() : 0;
                break;
            case ProgressDisplayMode.Sum:
                finalValue = progressArr.Length > 0 ? progressArr.Sum() : 0;
                break;
            case ProgressDisplayMode.First:
                finalValue = progressArr.Length > 0 ? progressArr.First() : 0;
                break;
        }

        return finalValue;
    }

    public bool IsCompleted()
    {
        bool result = false;
        switch(baseData.conditionLogic)
        {
            case ConditionLogic.AND:
                result = conditionArr.All(cond => cond.IsCompleted(baseData.targetValue));
                break;
            case ConditionLogic.OR:
                result = conditionArr.Any(cond => cond.IsCompleted(baseData.targetValue));
                break;
        }
        return result;
    }

    public string SaveState()
    {
        var arr = conditionArr.Select(cond => cond.SaveState()).ToArray();

        return JsonUtility.ToJson(new QuestWrapper { datas = arr });
    }

    public void LoadState(string saveJsonData)
    {
        if (string.IsNullOrEmpty(saveJsonData))
            return;

        var wrapper = JsonUtility.FromJson<QuestWrapper>(saveJsonData);

        if (wrapper == null)
            return;

        for(int i = 0; i < Mathf.Min(wrapper.datas.Length); i++)
        {
            conditionArr[i].LoadState(wrapper.datas[i]);
        }
    }

    [SerializeField]
    private class QuestWrapper
    {
        public string[] datas;
    }
}
