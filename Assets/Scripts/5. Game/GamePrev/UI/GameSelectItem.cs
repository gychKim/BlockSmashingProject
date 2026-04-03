using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class GameSelectItem : MonoBehaviour, IPoolable
{
    public int ID => id;
    private int id;
    public GameObject SelectImage => selectImage;
    [SerializeField]
    private GameObject selectImage;

    public Image ItemImage => itemImage;
    [SerializeField]
    private Image itemImage;

    public StringReactiveProperty CountRP;
    public TextMeshProUGUI CountText => countText;
    [SerializeField]
    private TextMeshProUGUI countText;

    public Button SelectButton => selectButton;
    [SerializeField]
    private Button selectButton;

    public void Init(ShopItemDataSO data, int count)
    {
        CountRP = new();
        CountRP
            .Subscribe(count =>
            {
                countText.text = count;
            }).AddTo(this);

        itemImage.sprite = data.Sprite;
        countText.text = count.ToString();
        id = data.ID;
        
        //id = data.ID;
        //itemName = data.Name;
        //description = data.Description;
        //image.sprite = data.Sprite;
        //price = data.Price;
        //itemType = data.ItemType;
    }

    public void OpenSelectUI()
    {
        selectImage.gameObject.SetActive(true);
    }

    public void CloseSelectUI()
    {
        selectImage.gameObject.SetActive(false);
    }
    public void Get()
    {
        
    }

    public void Release()
    {
        itemImage.sprite = null;
        countText.text = "";
        CountRP = null;
        id = -1;
    }

    public void Destroy()
    {
        itemImage.sprite = null;
        countText.text = "";
        CountRP = null;
        id = -1;
    }
}
