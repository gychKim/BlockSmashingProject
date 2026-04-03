using System;
using UniRx;

public class ShopUIPresenter : BasePresenter<ShopUIView, ShopUIModel>
{
    private Guid swapMainUIEventKey;


    public ShopUIPresenter(ShopUIView view, ShopUIModel model) : base(view, model)
    {
        swapMainUIEventKey = EventManager.Instance.Subscribe(UIEventType.SwapMainUI, ResetUI);
        
    }

    public override void Bind()
    {
        // 상점 활성화 버튼
        view.TempOpenShopButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.Open();
            }).AddTo(view.Disposables);

        // 아이템 상점 탭 버튼
        view.ItemTapButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.ActiveShop(ShopType.Item);
            }).AddTo(view.Disposables);

        
    }

    private void SetShopItem(ShopType type)
    {

    }

    /// <summary>
    /// SelectUI를 리셋시킨다.
    /// </summary>
    private void ResetUI()
    {
        view.ActiveShop(ShopType.Item);
    }


    public override void Destroy()
    {
        EventManager.Instance.Unsubscribe(UIEventType.SwapMainUI, swapMainUIEventKey);
    }
}
