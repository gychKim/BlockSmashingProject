using UnityEngine;

public class MainUIPresenter : IPresenter<MainUIView, MainUIModel>
{
    private MainUIView view;
    private MainUIModel model;
    public MainUIPresenter(MainUIView view, MainUIModel model)
    {
        this.view = view;
        this.model = model;
    }
    public void Bind()
    {
        
    }

    public void Destroy()
    {
        
    }
}
