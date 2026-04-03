using System;
using UniRx;

public class GamePrevPresenter : BasePresenter<GamePrevView, GamePrevModel>, IPresenter<GamePrevView, GamePrevModel>
{
    private Guid gameStartEventKey, gameReStartEventKey, endPrevDirectionEventKey;
    public GamePrevPresenter(GamePrevView view, GamePrevModel model) : base(view, model)
    {
        gameStartEventKey = EventManager.Instance.Subscribe(MainGameEventType.GameStart, StartMainGame);
        endPrevDirectionEventKey = EventManager.Instance.Subscribe(GameSceneEventType.EndPrevDirection, OpenPrevUI);

        gameReStartEventKey = EventManager.Instance.Subscribe(MainGameEventType.GameReStart, StartMainGame);
    }
    public override void Bind()
    {
        view.GameStartButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                // 메인게임 시작
                EventManager.Instance.Publish(MainGameEventType.GameStart);

            }).AddTo(view.Disposables);
    }

    /// <summary>
    /// 메인 게임 시작 시
    /// </summary>
    private void StartMainGame()
    {
        view.ClosePrevUI(); // View의 모든 UI Close
    }

    /// <summary>
    /// PrevUI를 활성화한다.
    /// </summary>
    private void OpenPrevUI()
    {
        view.OpenPrevUI();
    }

    public override void Destroy()
    {
        EventManager.Instance.Unsubscribe(MainGameEventType.GameStart, gameStartEventKey);
        EventManager.Instance.Unsubscribe(GameSceneEventType.EndPrevDirection, endPrevDirectionEventKey);
        EventManager.Instance.Unsubscribe(MainGameEventType.GameReStart, gameReStartEventKey);
    }
}
