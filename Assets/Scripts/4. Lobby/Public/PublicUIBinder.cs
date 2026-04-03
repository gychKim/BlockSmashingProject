using UnityEngine;

public class PublicUIBinder : BaseBinder<PublicUIView, PublicUIPresenter, PublicUIModel>
{
    protected override PublicUIModel CreateModel()
    {
        return new PublicUIModel();
    }

    protected override PublicUIPresenter CreatePresenter(PublicUIView view, PublicUIModel model)
    {
        return new PublicUIPresenter(view, model);
    }
}
