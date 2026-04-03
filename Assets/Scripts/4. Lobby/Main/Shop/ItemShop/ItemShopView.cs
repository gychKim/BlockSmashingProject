using UniRx;
using UnityEngine;

public class ItemShopView : BaseView
{
    /// <summary>
    /// Content의 Transform
    /// </summary>
    public Transform ContentTrans => contentTrans;
    [SerializeField]
    private Transform contentTrans;

    /// <summary>
    /// 구매 확인 UI
    /// </summary>
    public ItemShopPurchaseUI PurchaseUI => purchaseUI;
    [SerializeField]
    private ItemShopPurchaseUI purchaseUI;
}
