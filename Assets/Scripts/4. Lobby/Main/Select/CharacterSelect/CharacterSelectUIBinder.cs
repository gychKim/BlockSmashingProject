using UnityEngine;

public class CharacterSelectUIBinder : BaseBinder<CharacterSelectUIView, CharacterSelectUIPresenter, CharacterSelectUIModel>
{
    protected override CharacterSelectUIModel CreateModel()
    {
        return new CharacterSelectUIModel();
    }

    protected override CharacterSelectUIPresenter CreatePresenter(CharacterSelectUIView view, CharacterSelectUIModel model)
    {
        return new CharacterSelectUIPresenter(view, model);
    }
}
