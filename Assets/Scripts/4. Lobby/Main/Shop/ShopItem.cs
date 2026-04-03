using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour, IPoolable
{
    public Image Image => image;
    [SerializeField]
    private Image image;

    public int ID => id;
    private int id;

    public string ItemName => itemName;
    private string itemName;

    public string Description => description;
    private string description;

    public int Price => price;
    private int price;

    public ShopItemType ItemType => itemType;
    private ShopItemType itemType;

    public Button Button => button;
    [SerializeField]
    private Button button;

    public void Init(ShopItemDataSO data)
    {
        id = data.ID;
        itemName = data.Name;
        description = data.Description;
        image.sprite = data.Sprite;
        price = data.Price;
        itemType = data.ItemType;
    }

    /// <summary>
    /// Pool에서 가져올 때
    /// </summary>
    public void Get()
    {
        if (image == null)
            image = GetComponent<Image>();

        if (button == null)
            button = GetComponent<Button>();
    }

    /// <summary>
    /// Pool로 돌려보낼 때
    /// </summary>
    public void Release()
    {
        id = -1;
        description = itemName = "";
        image.sprite = null;
        price = 0;
        itemType = ShopItemType.None;
    }

    /// <summary>
    /// Pool에서 제거할 때
    /// </summary>
    public void Destroy()
    {
        id = -1;
        description = itemName = "";
        image = null;
        button = null;
        price = 0;
        itemType = ShopItemType.None;
    }
}
