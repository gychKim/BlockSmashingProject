using UnityEngine;

public class OutGameBinder : BaseBinder<OutGameView, OutGamePresenter, OutGameModel>
{
    protected override OutGameModel CreateModel()
    {
        return new OutGameModel();
    }

    protected override OutGamePresenter CreatePresenter(OutGameView view, OutGameModel model)
    {
        return new OutGamePresenter(view, model);
    }
}
