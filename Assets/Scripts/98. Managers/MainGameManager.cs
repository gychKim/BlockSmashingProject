using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

#if UNITY_EDITOR
using System.Diagnostics;
#endif

public class MainGameManager : Singleton<MainGameManager>, IDataInitialize
{
    private MainGameModel model;

    [SerializeField]
    private MainGameController gameController;

    public List<BlockItemData> CurrentApplyEffectList => currentApplyEffectList;
    private List<BlockItemData> currentApplyEffectList = new();

    public int NextStage => gameController.NextStage; // 다음 스테이지

    public int CurrentGameTime => gameController.CurrentGameTime.Value;
    public IObservable<int> CurrentGameTimeObv => gameController.CurrentGameTime.AsObservable();

    public IntReactiveProperty CurrentStage = new();/*=> gameController.CurrentStage.Value; // 현재 스테이지*/
    public IObservable<int> CurrentStageObv => gameController.CurrentStage.AsObservable();

    public float CurrentGoalScore => gameController.GoalScore.Value; // 현재 목표 점수
    public IObservable<float> CurrentGoalScoreObv => gameController.GoalScore.AsObservable();

    public float CurrentScore => gameController.CurrentScore.Value; // 현재 점수
    public IObservable<float> CurrentScoreObv => gameController.CurrentScore.AsObservable();

    public float CurrentCombo => gameController.CurrentCombo.Value; // 현재 콤보
    public IObservable<int> CurrentComboObv => gameController.CurrentCombo.AsObservable();

    public bool CurrentGameResult => gameController.CurrentGameResult;

    public int TotalGold => totalGold; // 획득한 최종 골드
    private int totalGold;

    public int TotalDia => totalDia; // 획득한 최종 다이아
    private int totalDia;

    public int HighestCombo => highestCombo; // 가장 높은 콤보
    private int highestCombo;

    public UniTaskCompletionSource LoadToken { get; } = new UniTaskCompletionSource();

    

#if UNITY_EDITOR
    public Stopwatch stopwatch = new Stopwatch();
#endif

    protected override void Awake()
    {
        IsDontDestroyOnLoad = false;

        base.Awake();

        CurrentStage.Value = GameManager.Instance.CurrentStage;

        LoadToken.TrySetResult();
    }

    public void SetModel(MainGameModel model)
    {
        this.model = model;
    }

    public MainGameModel GetModel()
    {
        return model;
    }

    //public void InitStage()
    //{
    //    totalGold = 0;
    //    totalDia = 0;
    //    highestCombo = 0;
    //}
    /// <summary>
    /// 게임 진행하면서 획득한 골드 추가
    /// </summary>
    /// <param name="value"></param>
    public void AddTotalGold(int value)
    {
        totalGold += value;
    }

    /// <summary>
    /// 획득한 최종 골드 초기화
    /// </summary>
    /// <param name="value"></param>
    public void ClearTotalGold()
    {
        totalGold = 0;
    }

    /// <summary>
    /// 게임 진행하면서 획득한 다이아 추가
    /// </summary>
    /// <param name="value"></param>
    public void AddTotalDia(int value)
    {
        totalDia += value;
    }

    /// <summary>
    /// 획득한 최종 다이아 초기화
    /// </summary>
    /// <param name="value"></param>
    public void ClearTotalDia()
    {
        totalDia = 0;
    }

    public void SetHighestCombo(int combo)
    {
        if(highestCombo < combo)
            highestCombo = combo;
    }

    /// <summary>
    /// 효과 리스트 추가
    /// </summary>
    /// <param name="effectID"></param>
    public bool AddShopItemEffect(int itemID)
    {
        var data = DBManager.Instance.GetItemData(itemID);

        if (currentApplyEffectList.Contains(data))
            return false;

        currentApplyEffectList.Add(data);
        return true;
    }

    private void OnDestroy()
    {
        ClearTotalGold();
        ClearTotalDia();

        model = null;

        if(!IsDontDestroyOnLoad)
            _instance = null;
    }
}
