using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class MainGameController : MonoBehaviour
{
    private MainGameModel model;

    private BlockFactory blockFactory; // 블록생성 팩토리
    private ItemController itemController;

    [SerializeField]
    private BlockContainer leftBlockContainer; // 좌측 블록 리스트
    [SerializeField]
    private BlockContainer rightBlockContainer; // 우측 블록 리스트

    #region 게임매니저에 있어야 할 것들
    [LabelColor("게임 데이터", 80, 188, 223)]
    [HeaderColor("스테이지 데이터", 0, 153, 0)]
    [ReadOnly] public IntReactiveProperty CurrentStage = new IntReactiveProperty(); // 현재 스테이지
    [ReadOnly] public FloatReactiveProperty GoalScore = new FloatReactiveProperty(); // 목표 점수

    [ReadOnly] public int NextStage = 2; // 다음 스테이지
    [ReadOnly] public int CurrentBaseBlockScore; // 스테이지 당 기본 블록 점수

    [HeaderColor("게임 상태 데이터", 242, 152, 134)]
    [ReadOnly] public IntReactiveProperty CurrentGameTime = new IntReactiveProperty(); // 게임 시간
    [ReadOnly] public FloatReactiveProperty CurrentScore = new FloatReactiveProperty(0); // 현재 총 점수
    [ReadOnly] public IntReactiveProperty CurrentCombo = new IntReactiveProperty(0); // 현재 콤보
    [ReadOnly] public FloatReactiveProperty CurrentComboTimer = new FloatReactiveProperty(0); // 콤보 제한시간 타이머

    [ReadOnly] public int ProgressGameTime; //  진행한 게임 총 시간
    [ReadOnly] public int CurrentBlockScore = 10; // 블록 당 획득하는 점수
    [ReadOnly] public bool CurrentGameResult; // 게임 결과
    [ReadOnly] public int CurrentGameTimeSpeed = 1; // 현재 시간 흐르는 속도
    [ReadOnly] public float CurrentScoreMultiplier = 1f; // 현재 점수 배율

    [HeaderColor("아이템 데이터", 171, 231, 158)]
    public IntReactiveProperty CurrentShieldCount => itemController.CurrentShieldCount; // 현재 실드 개수
    public FloatReactiveProperty CurrentFeverTimer => itemController.CurrentFeverTimer; // 피버 타이머
    public FloatReactiveProperty CurrentStopGameTimer => itemController.CurrentStopGameTimer;   // 게임 시간 정지 타이머
    public FloatReactiveProperty CurrentItemSpawnChanceTimer => itemController.CurrentItemSpawnChanceTimer; // 아이템 획득 확률 타이머

    private CancellationTokenSource gameTimerCancelToken; // 게임타이머Task의 CancelToken > 게임종료, 강제종료 시 사용
    private CancellationTokenSource comboTimerCancelToken; // 콤보 타이머 CancelToken
    #endregion

    [HeaderColor("아이템 데이터", 217, 217, 155)]
    private GameObject shieldEffect; // 실드 이펙트

    private IntTimerAsync gameTimerAsync;
    private FloatTimerAsync comboTimerAsync;

    #region 게임 상태 관련

    public BoolReactiveProperty isPause = new BoolReactiveProperty(false);
    public BoolReactiveProperty IsFever => itemController.isFever;
    public float ItemSpawnChance => itemController.ItemSpawnChance;
    #endregion

    private Guid startGameKey, restartKey, gamePauseKey, gameResumKey, useLeftBlockKey, useRightBlockKey; // 이벤트 고유 키 값

    private void Awake()
    {
        blockFactory = new BlockFactory(this); // 블록 팩토리 생성 > Model이 제거되지는 않으니, 최초 Model이 생성될 때 팩토리 생성
        itemController = new ItemController(this);

        leftBlockContainer.Init(this);
        rightBlockContainer.Init(this);

        startGameKey = EventManager.Instance.Subscribe(MainGameEventType.GameStart, StartGame);
        restartKey = EventManager.Instance.Subscribe(MainGameEventType.GameReStart, ReStartGame);
        gamePauseKey = EventManager.Instance.Subscribe(MainGameEventType.GamePause, GamePause);
        gameResumKey = EventManager.Instance.Subscribe(MainGameEventType.GameResum, GameResum);
        useLeftBlockKey = EventManager.Instance.Subscribe(MainGameEventType.LeftClick, UseLeftBlock);
        useRightBlockKey = EventManager.Instance.Subscribe(MainGameEventType.RightClick, UseRightBlock);

        CurrentStage.Value = GameManager.Instance.CurrentStage;
        //EventManager.Instance.Subscribe(MainGameEventType.N, SetNextStage);

    }

    private void Start()
    {
        model = MainGameManager.Instance.GetModel();

        model.Init(itemController);
        
        CurrentStage
            .Subscribe(stage =>
            {
                model.SetStage(stage);
                //EventManager.Instance.Publish(MainGameEventType.SetCurrentStage, stage);
            }).AddTo(gameObject);

        GoalScore
            .Subscribe(score =>
            {
                model.SetGoalScore(score);
            }).AddTo(gameObject);

        CurrentGameTime
            .Subscribe(time =>
            {
                model.SetGameTimer(time);
            }).AddTo(gameObject);

        CurrentScore
            .Subscribe(score =>
            {
                model.SetScore(score);
            }).AddTo(gameObject);

        CurrentCombo
            .Subscribe(value =>
            {
                model.SetCombo(value);

                MainGameManager.Instance.SetHighestCombo(value);

                if (value <= 0)
                {
                    itemController.RemoveAllModifier(EffectType.ScoreMultiplier);
                    return;
                }

                if (value % 10 == 0)
                {
                    itemController.AddScoreMultiplier(0.2f, -1f);
                }
            }).AddTo(gameObject);

        CurrentComboTimer
            .Subscribe(value =>
            {
                model.SetComboTimer(value);
            }).AddTo(gameObject);
    }

    /// <summary>
    /// Model 데이터 초기화
    /// </summary>
    /// <param name="ui"></param>
    public void Init()
    {
        Observable.Interval(TimeSpan.FromSeconds(1))
            .Subscribe(_ =>
            {
                ProgressGameTime++;
            });

        // 토큰 초기화
        gameTimerCancelToken = new CancellationTokenSource();

        // Timer클래스 초기화 > 초기화 할 때 마다 새로 생성해서 할당받도록
        gameTimerAsync = new IntTimerAsync(() => isPause.Value || CurrentGameTimeSpeed <= 0, gameTimerCancelToken);
        comboTimerAsync = new FloatTimerAsync(() => isPause.Value, comboTimerCancelToken);

        itemController.Init();
    }

    /// <summary>
    /// 메인게임 종료 시, 호출
    /// </summary>
    public void GameExit()
    {
        // 데이터 초기화
        ClearGameData(true);

        // 반응형 변수 전부 초기화
        CurrentStage.Dispose();
        GoalScore.Dispose();
        CurrentGameTime.Dispose();
        CurrentScore.Dispose();
        CurrentCombo.Dispose();

        // 외부 클래스 초기화
        itemController.Clear();
        blockFactory.Dispose();
    }

    /// <summary>
    /// 게임 일시정지
    /// </summary>
    public void GamePause()
    {
        isPause.Value = true;
    }

    /// <summary>
    /// 게임 재실행
    /// </summary>
    public void GameResum()
    {
        isPause.Value = false;
    }

    /// <summary>
    /// 다음 스테이지
    /// </summary>
    public void StartGame()
    {
        Init();

        ClearGameData();

        InitStageData();
        InitGameData();

        foreach(var item in MainGameManager.Instance.CurrentApplyEffectList)
        {
            itemController.ApplyShopItem(item);
        }
        
    }

    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void ReStartGame()
    {
        ClearGameData(); // 게임 데이터 Clear
        CurrentStage.Value = 1;
        NextStage = 2;
        //스테이지 데이터 초기화 후, 게임 데이터 초기화 해야한다.

        InitStageData(); // 스테이지 데이터 초기화(스테이지 1 데이터로 초기화)
    }

    /// <summary>
    /// 게임 데이터 초기화
    /// </summary>
    public void InitGameData()
    {
        for (int i = 0; i < 9; i++)
        {
            CreateBlock(); // 블록 생성 > 6개는 현재 화면 비례한 블록 사이즈에 딱 맞음 > 후에 블록 사이즈 변경되면 변경해야한다.
            if (i == 0)
                ReplaceBlock();
        }

        //ReplaceBlock(); // 리스트 0번째에 있는 블록을 선택 블록으로 이동
        ReplaceBlockList(); // 리스트 블록들 한칸 아래로 이동 > 리스트 순서 정렬

        // 게임 시작 혹은 을 눌러야 아래가 실행되어야 한다.
        StartGameTimer();
    }

    /// <summary>
    /// 스테이지 데이터 초기화
    /// </summary>
    private void InitStageData()
    {
        var stageData = DBManager.Instance.GetStageData(CurrentStage.Value);

        CurrentGameTime.Value = stageData.time;
        GoalScore.Value = stageData.goalScore;
        CurrentBaseBlockScore = stageData.blockScore;
        CurrentCombo.Value = 0;

        UpdateBlockScore();
    }

    /// <summary>
    /// 스테이지 값 초기화
    /// </summary>
    public void SetNextStage()
    {
        CurrentStage.Value = NextStage;
        NextStage++;
    }

    /// <summary>
    /// 게임 타이머 시작
    /// </summary>
    /// <param name="value"></param>
    /// <param name="duration"></param>
    public async void StartGameTimer()
    {
        // Token Cancel 실행
        try
        {
            var task = gameTimerAsync.Start(CurrentGameTime.Value);

            gameTimerAsync.Value
                .Subscribe(value =>
                {
                    CurrentGameTime.Value = value;
                });

            await task;
        }
        catch (OperationCanceledException) // GameTimer가 이 에러를 Catch하는 경우는, 대부분 게임 종료의 이유다.
        {
            DebugX.Log("GameTimer Cancel");
            return;
        }

        CurrentGameTime.Value = 0;
        GameEnd(); // 게임 종료
    }

    /// <summary>
    /// 게임 데이터 초기화
    /// </summary>
    public void ClearGameData(bool isDispose = false)
    {
        // 토큰 초기화
        gameTimerCancelToken?.Cancel();

        if (isDispose)
        {
            // 토큰 Remove
            gameTimerCancelToken = null;
            // Timer클래스 Remove
            gameTimerAsync = null;
            comboTimerAsync = null;
        }
        else
        {
            // 토큰 초기화
            gameTimerCancelToken = new CancellationTokenSource();

            // Timer클래스 초기화 > 초기화 할 때 마다 새로 생성해서 할당받도록
            gameTimerAsync = new IntTimerAsync(() => isPause.Value || CurrentGameTimeSpeed <= 0, gameTimerCancelToken);

            comboTimerAsync = new FloatTimerAsync(() => isPause.Value, comboTimerCancelToken);
        }

        // 게임 데이터 변수 초기화
        CurrentGameTime.Value = 0;
        GoalScore.Value = 0;
        CurrentBaseBlockScore = 0;
        CurrentScore.Value = 0;
        CurrentBlockScore = 0;
        CurrentCombo.Value = 0; // 현재 콤보 초기화
        CurrentComboTimer.Value = 0; // 콤보 타이머 초기화
        CurrentScoreMultiplier = 1; // 배율 아이템
        CurrentGameTimeSpeed = 1; // 게임 시간정지 아이템
        ProgressGameTime = 0; // 게임진행시간 초기화

        isPause.Value = false;

        // 블록 데이터 초기화
        itemController.Clear(isDispose);
        ClearBlockData();
    }

    /// <summary>
    /// 블록 데이터 초기화
    /// </summary>
    public void ClearBlockData()
    {
        ReturnAllBlock(); // 모든 블록을 제거

        leftBlockContainer.Clear();
        rightBlockContainer.Clear();

        //leftBlockQueue.Clear();
        //rightBlockQueue.Clear();
    }

    /// <summary>
    /// 좌측블록 사용 함수
    /// </summary>
    public void UseLeftBlock()
    {
        leftBlockContainer.UseBlock();

        //currentLeftBlockFrame.UseBlock(); // 블록 사용
        ReturnCurrentBlock();

        CreateBlock(); // 현재 블록을 사용하기 전, 미리 새로운 블록을 생성해준다. > Frame의 비활성화된 블록을 재활성화 한다.

        ReplaceBlock();
        ReplaceBlockList();

#if UNITY_EDITOR
        MainGameManager.Instance.stopwatch.Stop();
        DebugX.OrangeLog($"좌클릭 로직 종료 : {MainGameManager.Instance.stopwatch.Elapsed.TotalMilliseconds}");
        MainGameManager.Instance.stopwatch.Reset();
#endif
    }

    /// <summary>
    /// 우측블록 사용 함수
    /// </summary>
    public void UseRightBlock()
    {
        rightBlockContainer.UseBlock();

        //currentRightBlockFrame.UseBlock();

        ReturnCurrentBlock();

        CreateBlock();

        ReplaceBlock();
        ReplaceBlockList();

#if UNITY_EDITOR
        MainGameManager.Instance.stopwatch.Stop();
        DebugX.OrangeLog($"우클릭 로직 종료 : {MainGameManager.Instance.stopwatch.Elapsed.TotalMilliseconds}");
        MainGameManager.Instance.stopwatch.Reset();
#endif
    }

    #region 블록 아이템 관련

    /// <summary>
    /// 점수 증가
    /// </summary>
    /// <param name="value"></param>
    public void AddScore()
    {
        CurrentScore.Value += CurrentBlockScore;

        if (CurrentScore.Value >= GoalScore.Value)
        {
            GameEnd();
            return;
        }

        CurrentCombo.Value++; // 콤보 증가

        // 콤보가 진행중이라면 남은 시간 갱신
        if (comboTimerAsync != null && comboTimerAsync.IsRunning)
            comboTimerAsync.UpdateDuration(1f);
        // 아니라면 콤보 시작
        else
            SetComboTimer();
    }

    /// <summary>
    /// 점수 감소
    /// </summary>
    /// <param name="value"></param>
    public void RemoveScore()
    {
        if (itemController.UseShield()) // 실드가 존재하면, 실드 개수 감소 후 리턴
            return;

        CurrentScore.Value -= CurrentBlockScore * 5f; // 점수가 블록 점수의 5배를 감소시킨다.
        CurrentCombo.Value = 0; // 콤보 초기화

        comboTimerCancelToken?.Cancel(true); // 콤보 타이머 종료
        //scoreMultiplierCancelToken?.Cancel(true); // 콤보 타이머 종료

    }

    /// <summary>
    /// 게임 시간 설정
    /// </summary>
    /// <param name="value"></param>
    public void SetGameTimer(int value)
    {
        CurrentGameTime.Value += value;
        gameTimerAsync.UpdateDuration(CurrentGameTime.Value);
    }

    /// <summary>
    /// 콤보 타이머 설정
    /// </summary>
    /// <param name="value"></param>
    /// <param name="duration"></param>
    public async void SetComboTimer()
    {
        // Token Cancel 실행
        comboTimerCancelToken?.Cancel(true);
        comboTimerCancelToken = new CancellationTokenSource(); // Token생성

        comboTimerAsync = new FloatTimerAsync(() => isPause.Value, comboTimerCancelToken); // 게임정지 시 일시정지 및 Token 추가

        try
        {
            var task = comboTimerAsync.Start(1f);

            comboTimerAsync.Value
                .Subscribe(value =>
                {
                    CurrentComboTimer.Value = value;
                });

            await task;
        }
        catch (OperationCanceledException) // catch되는 경우는, 해당 아이템 지속시간 중 다시 한번 더 사용했거나, 스테이지 종료 및 게임 종료때이다.
        {
            DebugX.Log("Combo Cancel");
            return;
        }
        // return을 하지 않거나, finally에 초기화 로직을 수행하면, 아이템 재사용시 재사용 로직(아이템 적용) > Cancel로직 > finally(혹은 진행)로 들어와 아이템 초기화되어, 아이템 효과가 사라지게 된다.

        comboTimerAsync = null;
        CurrentComboTimer.Value = 0;
        CurrentCombo.Value = 0;
    }

    public void SetScoreMultiplier(float value)
    {
        CurrentScoreMultiplier = value;
        UpdateBlockScore();
    }

    /// <summary>
    /// 현재 블록 당 획득 점수 갱신 (현재 Base블록 점수 * 현재 점수 배율)
    /// </summary>
    public void UpdateBlockScore() => CurrentBlockScore = (int)(CurrentBaseBlockScore * CurrentScoreMultiplier);

    ///// <summary>
    ///// 게임시간 정지
    ///// </summary>
    ///// <param name="duration"></param>
    //public async void StopGameTimer(float duration)
    //{
    //    CurrentGameTimeSpeed.Value = 0;

    //    await itemController.StopGameTimer(duration);

    //    CurrentGameTimeSpeed.Value = 1;
    //}

    public void SetGameTimeSpeed(int value)
    {
        CurrentGameTimeSpeed = value;
    }

    /// <summary>
    /// 피버 적용
    /// </summary>
    /// <param name="duration"></param>
    public void ApplyFever()
    {
        // ClearGameData의 일부 > 기존 배치된 블록 초기화
        ClearBlockData();

        // InitGameData의 일부 > 블록 생성 > Fever 상태이므로, 모든 블록이 파워업 블록으로 생성된다.
        for (int i = 0; i < 7; i++) // 왜 7이지? 6이면 맨 위 한칸이 생성이 안됨;;
        {
            CreateBlock(); // 블록 생성 > 6개는 현재 화면 비례한 블록 사이즈에 딱 맞음 > 후에 블록 사이즈 변경되면 변경해야한다.
        }
        ReplaceBlock(); // 리스트 0번째에 있는 블록을 선택 블록으로 이동
        ReplaceBlockList(); // 리스트 블록들 한칸 아래로 이동 > 리스트 정렬


    }

    ///// <summary>
    ///// 실드 아이템 적용
    ///// </summary>
    //public void Shield()
    //{
    //    currentLeftBlockFrame.Shield();
    //    currentRightBlockFrame.Shield();
    //}
    ///// <summary>
    ///// 실드 아이템 제거
    ///// </summary>
    //public void RemoveShield()
    //{
    //    currentLeftBlockFrame.RemoveShield();
    //    currentRightBlockFrame.RemoveShield();
    //    //shieldEffect.SetActive(false);
    //}

    /// <summary>
    /// 좌/우 블록 리스트 블록 전환
    /// </summary>
    public void BlockSwap()
    {
        //// 좌우측 위치 변환
        //var rightChildList = new List<Transform>();
        //for (int i = 0; i < rightBlockListTransform.childCount; i++)
        //{
        //    rightChildList.Add(rightBlockListTransform.GetChild(i));
        //}
        //for (int i = 0; i < leftBlockListTransform.childCount; i++)
        //{
        //    leftBlockListTransform.GetChild(0).SetParent(rightBlockListTransform, false);
        //    rightChildList[i].SetParent(leftBlockListTransform, false);
        //}

        //ReplaceBlockList();

        //// 좌우측 데이터 변환
        //var tempQueue = new Queue<BlockObject>();
        //tempQueue = leftBlockQueue;
        //leftBlockQueue = rightBlockQueue;
        //rightBlockQueue = tempQueue;
    }

    #endregion

    /// <summary>
    /// 블록 생성 함수
    /// </summary>
    public void CreateBlock()
    {
        var (leftBlock, rightBlock) = blockFactory.CreateBlock();

        leftBlockContainer.SetBlock(leftBlock);
        rightBlockContainer.SetBlock(rightBlock);
    }

    #region 블록 리스트, 저장된 블록 관련 처리관련
    /// <summary>
    /// 리스트 0번째에 있는 블록을 현재 블록으로 전달한다.
    /// </summary>
    public void ReplaceBlock()
    {
        leftBlockContainer.ReplaceBlock();
        rightBlockContainer.ReplaceBlock();
    }

    /// <summary>
    /// 좌/우 블록 리스트 재정렬
    /// </summary>
    public void ReplaceBlockList()
    {
        leftBlockContainer.AlignBlock();
        rightBlockContainer.AlignBlock();
    }

    /// <summary>
    /// 모든 블록을 Pool에 넣는다.
    /// </summary>
    public void ReturnAllBlock()
    {
        ReturnBlockList();
        ReturnCurrentBlock();
    }

    /// <summary>
    /// 좌/우 블록리스트에 있는 블록들을 Pool에 넣는다.
    /// </summary>
    public void ReturnBlockList()
    {
        leftBlockContainer.ReturnAllBlock();
        rightBlockContainer.ReturnAllBlock();
    }

    /// <summary>
    /// 블록을 Pool에 넣는다.
    /// </summary>
    public void ReturnCurrentBlock()
    {
        leftBlockContainer.ReturnCurrentBlock();
        rightBlockContainer.ReturnCurrentBlock();
    }

    #endregion

    /// <summary>
    /// 현재 목표 점수(스테이지 시작할 때 설정되었음)보다 받은 점수가 크면 true
    /// </summary>
    /// <returns></returns>
    public bool IsGameClear()
    {
        return CurrentScore.Value >= GoalScore.Value;
    }

    /// <summary>
    /// 게임 종료 시
    /// </summary>
    private void GameEnd()
    {
        CurrentGameResult = IsGameClear() && CurrentStage.Value < 5;
        EventManager.Instance.Publish(MainGameEventType.GameEnd);

        gameTimerCancelToken?.Cancel(); // 게임 타이머 정지
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe(MainGameEventType.GameStart, startGameKey);
        EventManager.Instance.Unsubscribe(MainGameEventType.GameReStart, restartKey);
        EventManager.Instance.Unsubscribe(MainGameEventType.GamePause, gamePauseKey);
        EventManager.Instance.Unsubscribe(MainGameEventType.GameResum, gameResumKey);
        EventManager.Instance.Unsubscribe(MainGameEventType.LeftClick, useLeftBlockKey);
        EventManager.Instance.Unsubscribe(MainGameEventType.RightClick, useRightBlockKey);

        GameExit();
    }
}
