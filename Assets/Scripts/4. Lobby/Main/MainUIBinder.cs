using UnityEngine;

public class MainUIBinder : BaseBinder<MainUIView, MainUIPresenter, MainUIModel>
{

    protected override MainUIModel CreateModel()
    {
        return new MainUIModel();
    }

    protected override MainUIPresenter CreatePresenter(MainUIView view, MainUIModel model)
    {
        return new MainUIPresenter(view, model);
    }
}
