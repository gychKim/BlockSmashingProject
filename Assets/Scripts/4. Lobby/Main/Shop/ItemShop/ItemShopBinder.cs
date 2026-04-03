using UnityEngine;

public class ItemShopBinder : BaseBinder<ItemShopView, ItemShopPresenter, ItemShopModel>
{
    protected override ItemShopModel CreateModel()
    {
        return new ItemShopModel();
    }

    protected override ItemShopPresenter CreatePresenter(ItemShopView view, ItemShopModel model)
    {
        return new ItemShopPresenter(view, model);
    }
}