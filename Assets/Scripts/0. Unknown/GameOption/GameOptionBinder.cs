using UnityEngine;

public class GameOptionBinder : BaseBinder<GameOptionView, GameOptionPresenter, GameOptionModel>
{
    protected override GameOptionModel CreateModel()
    {
        return new GameOptionModel();
    }

    protected override GameOptionPresenter CreatePresenter(GameOptionView view, GameOptionModel model)
    {
        return new GameOptionPresenter(view, model);
    }
}