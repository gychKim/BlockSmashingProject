using UnityEngine;

public class StartSceneBinder : BaseBinder<StartSceneView, StartScenePresenter, StartSceneModel>
{
    protected override StartSceneModel CreateModel()
    {
        return new StartSceneModel();
    }

    protected override StartScenePresenter CreatePresenter(StartSceneView view, StartSceneModel model)
    {
        return new StartScenePresenter(view, model);
    }
}
