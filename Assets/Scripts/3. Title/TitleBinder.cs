using UnityEngine;

public class TitleBinder : BaseBinder<TitleView, TitlePresenter, TitleModel>
{
    [SerializeField]
    private TitleSceneInit initData;
    protected override TitleModel CreateModel()
    {
        return new TitleModel();   
    }

    protected override TitlePresenter CreatePresenter(TitleView view, TitleModel model)
    {
        return new TitlePresenter(view, model, initData);
    }
}
