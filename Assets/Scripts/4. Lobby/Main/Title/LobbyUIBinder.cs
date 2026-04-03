using UnityEngine;

public class LobbyUIBinder : BaseBinder<LobbyUIView, LobbyUIPresenter, LobbyUIModel>
{
    protected override LobbyUIModel CreateModel()
    {
        return new LobbyUIModel();
    }

    protected override LobbyUIPresenter CreatePresenter(LobbyUIView view, LobbyUIModel model)
    {
        return new LobbyUIPresenter(view, model);
    }
}
