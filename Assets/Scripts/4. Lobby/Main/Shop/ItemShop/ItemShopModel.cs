using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class ItemShopModel : BaseModel
{
    public ReactiveCollection<ShopItem> Items { get; } = new ReactiveCollection<ShopItem>();

    public override void Start()
    {
        Items.Clear();

        // 아이템 생성 > 아이템 상점 데이터를 토대로 지정된 아이템을 생성한다.
        var data = DBManager.Instance.GetShopData(ShopType.Item);

        // 아이템 상점이 지니고 있는 아이템 ID 리스트를 전부 생성
        foreach(int id in data.ItemIDList)
        {
            var itemData = DBManager.Instance.GetShopItemData(id);
            var item = PoolManager.Instance.Rent(PoolObjectType.ShopItem).GetComponent<ShopItem>();

            item.Init(itemData); // 아이템 초기화

            Items.Add(item);
        }
    }

    /// <summary>
    /// 아이템 구매 시도
    /// </summary>
    public async UniTask<bool> Purchase(ShopItem item)
    {
        // 구매 실패
        if (GameManager.Instance.CurrentGold.Value < item.Price)
        {
            return false;
        }

        GameManager.Instance.RemoveGold(item.Price);

        await SaveManager.Instance.ItemSaveManager.UpdatePurchase(item.ID, 1);

        return true;
    }

    public override void Destroy()
    {

    }
}
