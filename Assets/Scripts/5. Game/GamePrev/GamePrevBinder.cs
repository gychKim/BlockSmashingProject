using UnityEngine;

public class GamePrevBinder : BaseBinder<GamePrevView, GamePrevPresenter, GamePrevModel>
{
    protected override GamePrevModel CreateModel()
    {
        return new GamePrevModel();
    }

    protected override GamePrevPresenter CreatePresenter(GamePrevView view, GamePrevModel model)
    {
        return new GamePrevPresenter(view, model);
    }
}
