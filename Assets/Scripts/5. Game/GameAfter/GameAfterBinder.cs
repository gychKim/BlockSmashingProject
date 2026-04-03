using UnityEngine;

public class GameAfterBinder : BaseBinder<GameAfterView, GameAfterPresenter, GameAfterModel>
{
    protected override GameAfterModel CreateModel()
    {
        return new GameAfterModel();
    }

    protected override GameAfterPresenter CreatePresenter(GameAfterView view, GameAfterModel model)
    {
        return new GameAfterPresenter(view, model);
    }
}
