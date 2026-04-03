using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIView : BaseView
{
    /// <summary>
    /// 상점 열기 버튼(임시)
    /// </summary>
    public Button TempOpenShopButton => tempOpenShopButton;
    [SerializeField]
    private Button tempOpenShopButton;

    /// <summary>
    /// 아이템 탭 버튼
    /// </summary> 
    public Button ItemTapButton => itemTapButton;
    [SerializeField]
    private Button itemTapButton;

    /// <summary>
    /// 상점
    /// </summary>
    public GameObject ShopUI => shopUI;
    [SerializeField]
    private GameObject shopUI;

    /// <summary>
    /// 캐릭터 상점
    /// </summary>
    public GameObject CharacterShopUI => characterShopUI;
    [SerializeField]
    private GameObject characterShopUI;

    /// <summary>
    /// 스킨 상점
    /// </summary>
    public GameObject SkinShopUI => skinShopUI;
    [SerializeField]
    private GameObject skinShopUI;

    /// <summary>
    /// 아이템 상점
    /// </summary>
    public GameObject ItemShopUI => itemShopUI;
    [SerializeField]
    private GameObject itemShopUI;

    /// <summary>
    /// ShopUI 활성화
    /// </summary>
    public void Open()
    {
        characterShopUI.SetActive(true);
        skinShopUI.SetActive(false);
        itemShopUI.SetActive(false);

        shopUI.SetActive(true);
    }

    /// <summary>
    /// ShopUI 리셋 == 닫기 == 화면전환 시
    /// </summary>
    public void ResetUI()
    {
        characterShopUI.SetActive(false);
        skinShopUI.SetActive(false);
        itemShopUI.SetActive(false);

        shopUI.SetActive(false);
    }
    public void ActiveShop(ShopType type)
    {
        characterShopUI.SetActive(type == ShopType.Character);
        skinShopUI.SetActive(type == ShopType.Skin);
        itemShopUI.SetActive(type == ShopType.Item);

        shopUI.SetActive(true);

    }
}
