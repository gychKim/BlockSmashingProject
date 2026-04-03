using UnityEngine;

public class GameItemSelectBinder : BaseBinder<GameItemSelectView, GameItemSelectPresenter, GameItemSelectModel>
{
    protected override GameItemSelectModel CreateModel()
    {
        return new GameItemSelectModel();
    }

    protected override GameItemSelectPresenter CreatePresenter(GameItemSelectView view, GameItemSelectModel model)
    {
        return new GameItemSelectPresenter(view, model);
    }
}
