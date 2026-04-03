using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameItemSelectView : BaseView
{
    public Transform ContentTrans => contentTrans;
    [SerializeField]
    private Transform contentTrans;

    public GameObject ItemInfoUI => itemInfoUI;
    [SerializeField]
    private GameObject itemInfoUI;

    public Image ItemInfoImage => itemInfoImage;
    [SerializeField]
    private Image itemInfoImage;

    public TextMeshProUGUI ItemNameText => itemNameText;
    [SerializeField]
    private TextMeshProUGUI itemNameText;

    public TextMeshProUGUI ItemInfoText => itemInfoText;
    [SerializeField]
    private TextMeshProUGUI itemInfoText;

    public TextMeshProUGUI ItemCountText => itemCountText;
    [SerializeField]
    private TextMeshProUGUI itemCountText;

    /// <summary>
    /// 아이템 사용 버튼
    /// </summary>
    public Button UseButton => useButton;
    [SerializeField]
    private Button useButton;

    public void AddItem(GameObject item)
    {
        item.transform.SetParent(contentTrans);
    }

    public void SetItem(ShopItemDataSO data, int count)
    {
        itemInfoImage.sprite = data.Sprite;
        itemNameText.text = data.Name;
        itemInfoText.text = data.Description;
        itemCountText.text = $"소지 개수 : {count.ToString()}";

        OpenInfoUI();
        useButton.gameObject.SetActive(true);
    }

    public void OpenInfoUI()
    {
        itemInfoUI.SetActive(true);
    }

    public void CloseInfoUI()
    {
        itemInfoUI.SetActive(false);
    }

    /// <summary>
    /// 사용 버튼 비활성화
    /// </summary>
    public void CloseUseButton()
    {
        useButton.gameObject.SetActive(false);
    }
}
