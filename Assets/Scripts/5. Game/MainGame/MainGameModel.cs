using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class MainGameModel : IModel
{
    private ItemController itemController;
    [LabelColor("게임 데이터", 80, 188, 223)]
    [HeaderColor("스테이지 데이터", 0, 153, 0)]
    [ReadOnly] public IntReactiveProperty CurrentStage = new IntReactiveProperty(1); // 현재 스테이지
    [ReadOnly] public IntReactiveProperty NextStage = new IntReactiveProperty(2); // 다음 스테이지
    [ReadOnly] public FloatReactiveProperty GoalScore = new FloatReactiveProperty(); // 목표 점수
    [ReadOnly] public IntReactiveProperty CurrentBaseBlockScore = new IntReactiveProperty(); // 스테이지 당 기본 블록 점수

    [HeaderColor("게임 상태 데이터", 242, 152, 134)]
    [ReadOnly] public IntReactiveProperty ProgressGameTime = new IntReactiveProperty(); //  진행한 게임 총 시간
    [ReadOnly] public IntReactiveProperty CurrentGameTime = new IntReactiveProperty(); // 게임 시간
    [ReadOnly] public FloatReactiveProperty CurrentScore = new FloatReactiveProperty(0); // 현재 총 점수
    [ReadOnly] public IntReactiveProperty CurrentBlockScore = new IntReactiveProperty(10); // 블록 당 획득하는 점수
    [ReadOnly] public IntReactiveProperty CurrentCombo = new IntReactiveProperty(0); // 현재 콤보
    [ReadOnly] public FloatReactiveProperty CurrentComboTimer = new FloatReactiveProperty(); // 콤보 제한시간 타이머
    [ReadOnly] public BoolReactiveProperty CurrentGameResult = new BoolReactiveProperty(); // 게임 결과
    [ReadOnly] public IntReactiveProperty CurrentGameTimeSpeed = new IntReactiveProperty(1); // 현재 시간 흐르는 속도
    [ReadOnly] public FloatReactiveProperty CurrentScoreMultiplier = new FloatReactiveProperty(1f); // 현재 점수 배율

    public IntReactiveProperty CurrentShieldCount => itemController.CurrentShieldCount; // 현재 실드 개수
    public FloatReactiveProperty CurrentFeverTimer => itemController.CurrentFeverTimer; // 피버 타이머
    public FloatReactiveProperty CurrentStopGameTimer => itemController.CurrentStopGameTimer;   // 게임 시간 정지 타이머
    public FloatReactiveProperty CurrentItemSpawnChanceTimer => itemController.CurrentItemSpawnChanceTimer; // 아이템 획득 확률 타이머

    public MainGameModel()
    {
        MainGameManager.Instance.SetModel(this);
    }

    public void Start()
    {
        
    }

    public void Init(ItemController itemController)
    {
        this.itemController = itemController;
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
        NextStage.Dispose();
        GoalScore.Dispose();
        CurrentBaseBlockScore.Dispose();
        ProgressGameTime.Dispose();
        CurrentGameTime.Dispose();
        CurrentScore.Dispose();
        CurrentBlockScore.Dispose();
        CurrentCombo.Dispose();
        CurrentComboTimer.Dispose();
        CurrentGameResult.Dispose();
        CurrentGameTimeSpeed.Dispose();
        CurrentScoreMultiplier.Dispose();
    }

    /// <summary>
    /// 스테이지 값 초기화
    /// </summary>
    public void SetStage(int stage)
    {
        CurrentStage.Value = stage;
        //CurrentStage.Value = NextStage.Value;
        //NextStage.Value++;
    }

    /// <summary>
    /// 게임 데이터 초기화
    /// </summary>
    public void ClearGameData(bool isDispose = false)
    {
        // 게임 데이터 변수 초기화
        CurrentGameTime.Value = 0;
        GoalScore.Value = 0;
        CurrentBaseBlockScore.Value = 0;
        CurrentScore.Value = 0;
        CurrentBlockScore.Value = 0;
        CurrentCombo.Value = 0; // 현재 콤보 초기화
        CurrentComboTimer.Value = 0; // 콤보 타이머 초기화
        CurrentScoreMultiplier.Value = 1; // 배율 아이템
        CurrentGameTimeSpeed.Value = 1; // 게임 시간정지 아이템
        ProgressGameTime.Value = 0; // 게임진행시간 초기화
    }

    ///// <summary>
    ///// 점수 변경
    ///// </summary>
    ///// <param name="value"></param>
    public void SetScore(float value)
    {
        CurrentScore.Value = value;
    }

    public void SetGoalScore(float score)
    {
        GoalScore.Value = score;
    }


    /// <summary>
    /// 게임 시간 설정
    /// </summary>
    /// <param name="value"></param>
    public void SetGameTimer(int value)
    {
        CurrentGameTime.Value = value;
    }

    public void SetCombo(int combo)
    {
        CurrentCombo.Value = combo;
    }

    public void SetComboTimer(float value)
    {
        CurrentComboTimer.Value = value;
    }

    public void SetGameTimeSpeed(int value)
    {
        CurrentGameTimeSpeed.Value = value;
    }

    public void Destroy()
    {
        GameExit();
    }
}
