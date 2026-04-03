using UnityEngine;

public class SelectUIBinder : BaseBinder<SelectUIView, SelectUIPresenter, SelectUIModel>
{
    protected override SelectUIModel CreateModel()
    {
        return new SelectUIModel();
    }

    protected override SelectUIPresenter CreatePresenter(SelectUIView view, SelectUIModel model)
    {
        return new SelectUIPresenter(view, model);
    }
}
