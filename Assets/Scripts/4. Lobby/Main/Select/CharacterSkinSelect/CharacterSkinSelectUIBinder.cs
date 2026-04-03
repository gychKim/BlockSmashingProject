using UnityEngine;

public class CharacterSkinSelectUIBinder : BaseBinder<CharacterSkinSelectUIView, CharacterSkinSelectPresenter, CharacterSkinSelectModel>
{
    protected override CharacterSkinSelectModel CreateModel()
    {
        return new CharacterSkinSelectModel();
    }

    protected override CharacterSkinSelectPresenter CreatePresenter(CharacterSkinSelectUIView view, CharacterSkinSelectModel model)
    {
        return new CharacterSkinSelectPresenter(view, model);
    }
}
