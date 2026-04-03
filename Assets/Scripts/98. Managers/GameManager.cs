using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;

public class GameManager : Singleton<GameManager>, IDataAwake, IDataStart
{
    public int CurrentCharacterID => currentCharacterID;
    [SerializeField, ReadOnly]
    private int currentCharacterID; // 현재 선택된 캐릭터 ID
    public int CurrentBlockID => currentBlockID;
    private int currentBlockID; // 현재 선택된 블록 ID
    public IReadOnlyReactiveProperty<int> CurrentGold => currentGold; // 현재 소지중인 골드 > 직접 변경 불가능, 반드시 Add or Remove 메소드를 이용하도록
    [SerializeField, ReadOnly]
    private IntReactiveProperty currentGold;

    public IReadOnlyReactiveProperty<int> CurrentDia => currentDia; // 현재 소지중인 다이아
    [SerializeField, ReadOnly]
    private IntReactiveProperty currentDia;

    public int CurrentStage { get; set; }

    private Guid applyCharacterEventKey, applyCharacterSkinEventKey, applyBlockEventKey; // 이벤트 Key

    public UniTask DataAwakeAsync(CancellationToken cancelToken)
    {
        applyCharacterEventKey = EventManager.Instance.Subscribe<UIEventType, int>(UIEventType.ApplyCharacter, ApplyCharacter);
        applyCharacterSkinEventKey = EventManager.Instance.Subscribe<UIEventType, int>(UIEventType.ApplySkin, ApplyCharacterSkin);
        applyBlockEventKey = EventManager.Instance.Subscribe<UIEventType, int>(UIEventType.ApplyBlock, ApplyBlock);

        return UniTask.CompletedTask;
    }

    public async UniTask DataStartAsync(CancellationToken cancelToken)
    {
        await DataLoad();

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
            .Subscribe(_ =>
            {
                DebugX.Log("OpenExitUI");
                EventManager.Instance.Publish(UIEventType.OpenGameExitUI);
            })
            .AddTo(this);
    }

    /// <summary>
    /// 데이터 로드 및 초기화
    /// </summary>
    /// <returns></returns>
    private async UniTask DataLoad()
    {
        // 게임 상태 데이터를 받는다.
        var gameStateData = await SaveManager.Instance.GameStateSaveManager.GetReadOnly();

        // 현재 캐릭터 ID 등록
        currentCharacterID = gameStateData.currentCharacterID;

        // 현재 블록 ID 등록
        currentBlockID = gameStateData.currentBlockID;

        // 현재 골드 등록
        currentGold.Value = gameStateData.currentGold;

        // 현재 다이아 등록
        currentDia.Value = gameStateData.currentDia;
    }

    /// <summary>
    /// 결정된 캐릭터의 ID를 가져온다.
    /// </summary>
    /// <param name="id"></param>
    private async void ApplyCharacter(int id)
    {
        currentCharacterID = id;
        await SaveManager.Instance.GameStateSaveManager.UpdateCurrentCharacterID(id); // 변경된 캐릭터 ID 저장
    }

    /// <summary>
    /// 결정된 캐릭터 스킨의 Number를 가져온다.
    /// </summary>
    /// <param name="skinNum"></param>
    private async void ApplyCharacterSkin(int skinNum)
    {
        await SaveManager.Instance.CharacterSaveManager.UpdateSkinIndex(CurrentCharacterID, skinNum);
        //ApplyCharacterSkinAsync(skinNum).Forget();
    }
    /// <summary>
    /// 결정된 캐릭터 스킨으로 데이터를 갱신한다.
    /// </summary>
    /// <param name="skinNum"></param>
    private async UniTask ApplyCharacterSkinAsync(int skinNum)
    {
        await SaveManager.Instance.CharacterSaveManager.UpdateSkinIndex(CurrentCharacterID, skinNum);
    }

    /// <summary>
    /// 결정된 블록의 ID를 가져온다.
    /// </summary>
    /// <param name="id"></param>
    private async void ApplyBlock(int blockID)
    {
        currentBlockID = blockID;
        await SaveManager.Instance.GameStateSaveManager.UpdateBlockID(blockID); // 변경된 블록 ID 저장

    }

    /// <summary>
    /// 골드 추가
    /// </summary>
    /// <param name="value"></param>
    public async void AddGold(int value)
    {
        currentGold.Value += value;

        await SaveManager.Instance.GameStateSaveManager.UpdateGold(currentGold.Value);

        // 메인 게임을 진행 중이라면
        if (MainGameManager.Instance != null)
        {
            MainGameManager.Instance.AddTotalGold(value);
        }
    }

    /// <summary>
    /// 골드 감소
    /// </summary>
    /// <param name="value"></param>
    public async void RemoveGold(int value)
    {
        currentGold.Value -= value;

        await SaveManager.Instance.GameStateSaveManager.UpdateGold(currentGold.Value);
    }

    /// <summary>
    /// 골드 추가
    /// </summary>
    /// <param name="value"></param>
    public async void AddDia(int value)
    {
        currentDia.Value += value;

        await SaveManager.Instance.GameStateSaveManager.UpdateDia(currentDia.Value);

        // 메인 게임을 진행 중이라면
        if (MainGameManager.Instance != null)
        {
            MainGameManager.Instance.AddTotalDia(value);
        }
    }

    /// <summary>
    /// 골드 감소
    /// </summary>
    /// <param name="value"></param>
    public async void RemoveDia(int value)
    {
        currentDia.Value -= value;

        await SaveManager.Instance.GameStateSaveManager.UpdateDia(currentDia.Value);
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void GameExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe(UIEventType.ApplyCharacter, applyCharacterEventKey);
        EventManager.Instance.Unsubscribe(UIEventType.ApplySkin, applyCharacterSkinEventKey);
        EventManager.Instance.Unsubscribe(UIEventType.ApplyBlock, applyBlockEventKey);
    }
}
