
public abstract class BasePresenter<TView, TModel> : IPresenter<TView, TModel> where TView : IUIView
{
    protected TView view;
    protected TModel model;

    public BasePresenter(TView view, TModel model)
    {
        this.view = view;
        this.model = model;
    }

    public abstract void Bind();

    public abstract void Destroy();
}
