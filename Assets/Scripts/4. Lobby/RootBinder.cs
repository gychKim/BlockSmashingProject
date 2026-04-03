using UnityEngine;

public class RootBinder : BaseBinder<RootView, RootPresenter, RootModel>
{
    protected override RootModel CreateModel()
    {
        return new RootModel();
    }
    protected override RootPresenter CreatePresenter(RootView view, RootModel model)
    {
        return new RootPresenter(view, model);
    }
}
