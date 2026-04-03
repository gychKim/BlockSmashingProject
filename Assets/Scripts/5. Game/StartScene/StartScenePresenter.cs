using System;
using UniRx;

public class StartScenePresenter : BasePresenter<StartSceneView, StartSceneModel>
{
    private Guid startSceneEventKey, startPrevDirectionEventKey;

    public StartScenePresenter(StartSceneView view, StartSceneModel model) : base(view, model)
    {
        startSceneEventKey = EventManager.Instance.Subscribe(GameSceneEventType.StartScene, StartScene);
        startPrevDirectionEventKey = EventManager.Instance.Subscribe(GameSceneEventType.StartPrevDirection, StartPrevDirection);
    }

    public override void Bind()
    {
        MainGameManager.Instance.CurrentStageObv
            .Subscribe(value =>
            {
                var data = DBManager.Instance.GetStageData(value);
                view.SetStage(value);
                view.SetStageName(data.stageName);
            }).AddTo(view.Disposables);
    }

    private void StartScene()
    {
        view.PlayAnimation();
    }

    private void StartPrevDirection()
    {
        view.ActiveUI(false);
    }

    public override void Destroy()
    {
         EventManager.Instance.Unsubscribe(GameSceneEventType.StartScene, startSceneEventKey);
         EventManager.Instance.Unsubscribe(GameSceneEventType.StartPrevDirection, startPrevDirectionEventKey);
    }
}
