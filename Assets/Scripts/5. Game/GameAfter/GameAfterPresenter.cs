using System;
using UniRx;
using UnityEngine;

public class GameAfterPresenter : BasePresenter<GameAfterView, GameAfterModel>, IPresenter<GameAfterView, GameAfterModel>
{
    private Guid gameStartEventKey, endAfterDirectionEventKey;

    private int currentStage;
    public GameAfterPresenter(GameAfterView view, GameAfterModel model) : base(view, model)
    {
        gameStartEventKey = EventManager.Instance.Subscribe(MainGameEventType.GameReStart, StartMainGame);
        endAfterDirectionEventKey = EventManager.Instance.Subscribe(GameSceneEventType.EndAfterDirection, EndDirection);
    }

    public override void Bind()
    {
        view.NextStageButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.NextStageUI.SetActive(false);
                EventManager.Instance.Publish(MainGameEventType.NextStage); // 다음 스테이지로 진입
                EventManager.Instance.Publish(GameSceneEventType.StartPrevDirection); // 게임 시작 전 연출 시작
            }).AddTo(view.Disposables);

        view.RestartButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.GameResultUI.SetActive(false);

                MainGameManager.Instance.ClearTotalGold(); // 획득한 골드 데이터 초기화
                MainGameManager.Instance.ClearTotalDia(); // 획득한 다이아 데이터 초기화

                EventManager.Instance.Publish(MainGameEventType.GameReStart); // 게임 재시작
                EventManager.Instance.Publish(GameSceneEventType.StartPrevDirection); // 게임 시작 전 연출 시작
            }).AddTo(view.Disposables);

        view.TitleButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.NextStageUI.SetActive(false);
                SceneManagerEX.Instance.LoadScene("LobbyScene");
                //EventManager.Instance.Publish(MainGameEventType.GameExit);

            }).AddTo(view.Disposables);

        view.TitleButton1
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                //model.GameResum();
                view.GameResultUI.SetActive(false);

                SceneManagerEX.Instance.LoadScene("LobbyScene");
                //EventManager.Instance.Publish(MainGameEventType.GameExit);

            }).AddTo(view.Disposables);

        MainGameManager.Instance.CurrentStage
            .Subscribe(stage =>
            {
                currentStage = stage;

                view.FinalStageText.text = $"최종 스테이지 : {currentStage}";
            }).AddTo(view.Disposables);

        MainGameManager.Instance.CurrentScoreObv
            .Subscribe(score =>
            {
                    view.FinalBlockScoreText.text = $"획득한 점수 : {score}";
                    view.FinalBlockScoreText1.text = $"획득한 점수 : {score}";
            }).AddTo(view.Disposables);

    }

    /// <summary>
    /// 블록 파괴 연출 종료 시
    /// </summary>
    /// <param name="result"></param>
    private void EndDirection()
    {
        model.EndDirection(); // Model에게 정보 전달

        // 한 UI에 타이틀, 다음 스테이지, 재시작 UI를 만들고, 상황에 따라 다음 스테이지 or 재시작 버튼을 활성화 시키게 만들자.
        if(MainGameManager.Instance.CurrentGameResult)
        {
            var stageReward = DBManager.Instance.GetStageData(currentStage).reward;

            if (stageReward == null)
                return;

            int gold = stageReward.gold + currentStage * MainGameManager.Instance.CurrentGameTime;
            view.GetGoldText.gameObject.SetActive(gold > 0);
            view.GetGoldText.text = $"골드 : {gold.ToString()} 획득"; // 여기에 적는게 싫으면 view.NextStageUI.OnEnableObservable을 사용하자?

            view.GetDiaText.gameObject.SetActive(stageReward.dia > 0);
            view.GetDiaText.text = $"다이아 : {stageReward.dia.ToString()} 획득";

            EventManager.Instance.Publish(QuestEventType.ComboChanged, MainGameManager.Instance.HighestCombo); // 콤보 퀘스트 갱신

            view.NextStageUI.SetActive(true);
        }
        else
        {
            //view.TotalGoldText.text = $"최종 골드 : {MainGameManager.Instance.TotalGold}"; // 여기에 적는게 싫으면 view.GameResultUI.OnEnableObservable을 사용하자?
            //view.TotalDiaText.text = $"최종 다이아 : {MainGameManager.Instance.TotalDia}";

            view.GameResultUI.SetActive(true);
        }
    }

    /// <summary>
    /// 메인 게임 시작 시
    /// </summary>
    private void StartMainGame()
    {
        view.StartMainGame(); // View의 모든 UI Close
    }

    public override void Destroy()
    {
        EventManager.Instance.Unsubscribe(MainGameEventType.GameReStart, gameStartEventKey);
        EventManager.Instance.Unsubscribe(GameSceneEventType.EndAfterDirection, endAfterDirectionEventKey);
    }
}
