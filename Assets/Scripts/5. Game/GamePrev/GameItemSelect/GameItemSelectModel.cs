using UniRx;
using UnityEngine;

public class GameItemSelectModel : BaseModel
{
    public ReactiveCollection<GameSelectItem> Items { get; } = new ReactiveCollection<GameSelectItem>();

    public GameSelectItem SelectedItem => selectedItem;
    private GameSelectItem selectedItem;
    public override async void Start()
    {
        Items.Clear();

        // 아이템 생성 > 아이템 상점 데이터를 토대로 지정된 아이템을 생성한다.
        var data = DBManager.Instance.GetShopData(ShopType.Item);

        // 아이템 상점이 지니고 있는 아이템 ID 리스트를 전부 생성
        foreach (int id in data.ItemIDList)
        {
            var itemData = DBManager.Instance.GetShopItemData(id);
            var item = PoolManager.Instance.Rent(PoolObjectType.GameSelectItem).GetComponent<GameSelectItem>();

            var saveData = await SaveManager.Instance.ItemSaveManager.GetReadOnly(id);

            if (saveData == null)
                continue;

            item.Init(itemData, saveData.itemCount); // 아이템 초기화

            Items.Add(item);
        }
    }

    /// <summary>
    /// 아이템 선택되었을 시 처리
    /// </summary>
    /// <param name="id"></param>
    public void SelectItem(GameSelectItem item)
    {
        foreach(var itemData in Items)
        {
            if (itemData == item)
                itemData.OpenSelectUI();
            else
                itemData.CloseSelectUI();
        }

        selectedItem = item;
    }

    /// <summary>
    /// 아이템 선택 해제 > 보통 사용 후 호출
    /// </summary>
    public void ResetSelect()
    {
        selectedItem.CloseSelectUI();
        selectedItem = null;
    }

    public override void Destroy()
    {
        
    }
}
