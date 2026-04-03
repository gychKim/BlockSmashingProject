using UnityEngine;

public class ShopUIBinder : BaseBinder<ShopUIView, ShopUIPresenter, ShopUIModel>
{
    protected override ShopUIModel CreateModel()
    {
        return new ShopUIModel();
    }
    protected override ShopUIPresenter CreatePresenter(ShopUIView view, ShopUIModel model)
    {
        return new ShopUIPresenter(view, model);
    }
}
