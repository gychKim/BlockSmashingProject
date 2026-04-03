using UnityEngine;
using UniRx;
using System;

public class ItemShopPresenter : BasePresenter<ItemShopView, ItemShopModel>
{
    public ItemShopPresenter(ItemShopView view, ItemShopModel model) : base(view, model)
    {
    }

    public override void Bind()
    {
        // 기존에 넣어져 있던 데이터 Binding
        foreach(var item in model.Items)
        {
            ItemBinding(item);
        }

        // 아이템 생성 및 추가될 시 > 이런 일이 어지간 하면 없지만 혹시나 모르니까
        model.Items
            .ObserveAdd()
            .Subscribe(item =>
            {
                ItemBinding(item.Value);

            }).AddTo(view.Disposables);
    }

    /// <summary>
    /// Item의 데이터 Binding
    /// </summary>
    /// <param name="item"></param>
    private void ItemBinding(ShopItem item)
    {
        item.transform.SetParent(view.ContentTrans); // 아이템을 Content에 넣는다.

        item.transform.localScale = Vector3.one; // 아이템의 스케일 조정

        item.Button
            .OnClickAsObservable()
            .Subscribe(async _ =>
            {
                view.PurchaseUI.Open();
                var result = await view.PurchaseUI.SetData(item); // 데이터 설정

                // 구매 클릭
                if (result)
                {
                    var purchase = await model.Purchase(item);

                    // 구매 성공 시
                    if (purchase)
                    {
                        view.PurchaseUI.Close(); // 닫기
                    }
                    // 구매 실패 시
                    else
                    {
                        view.PurchaseUI.OpenFailedPurchaseUI(); // 구매 실패 UI 활성화
                    }
                }
                // 돌아가기 클릭
                else
                {
                    view.PurchaseUI.Close();
                }

            }).AddTo(view.Disposables);
    }

    public override void Destroy()
    {
        
    }
}
