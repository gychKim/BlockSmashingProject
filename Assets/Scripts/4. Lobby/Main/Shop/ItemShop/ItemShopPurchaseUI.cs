using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopPurchaseUI : MonoBehaviour
{
    [SerializeField]
    private GameObject purchaseUI;

    private int itemID;

    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private TextMeshProUGUI itemName;

    [SerializeField]
    private TextMeshProUGUI itemDescription;

    [SerializeField]
    private TextMeshProUGUI itemPrice;

    [SerializeField]
    private Button acceptButton;

    [SerializeField]
    private Button rejectButton;

    [SerializeField]
    private GameObject failedPurchaseUI; // 구매 실패 UI

    [SerializeField]
    private Button confirmButton; // 구매 실패 UI 닫기 버튼

    private UniTaskCompletionSource<bool> tcs = new UniTaskCompletionSource<bool>();

    private void Start()
    {
        acceptButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                // 구매 클릭
                tcs.TrySetResult(true);
            }).AddTo(gameObject);

        rejectButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                // 돌아가기 클릭
                tcs.TrySetResult(false);
            }).AddTo(gameObject);

        // 구매 실패 UI 닫기버튼
        confirmButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                Close(); // UI 닫기
            }).AddTo(gameObject);

        // UI는 반드시 종료하도록
        if(purchaseUI.activeSelf)
            purchaseUI.SetActive(false);

        if (failedPurchaseUI.activeSelf)
            failedPurchaseUI.SetActive(false);
    }
    public async UniTask<bool> SetData(ShopItem data)
    { 
        itemID = data.ID;
        itemImage.sprite = data.Image.sprite;
        itemName.text = data.ItemName;
        itemDescription.text = data.Description;
        itemPrice.text = $"가격 : {data.Price}";

        tcs = new UniTaskCompletionSource<bool>(); // tcs 새로 할당

        var result = await tcs.Task;
        return result;
    }

    public void Open()
    {
        purchaseUI.SetActive(true);
    }

    public void Close()
    {
        purchaseUI.SetActive(false);
        CloseFailedPurchaseUI();
    }

    public void OpenFailedPurchaseUI()
    {
        failedPurchaseUI.SetActive(true);
        
    }

    public void CloseFailedPurchaseUI()
    {
        failedPurchaseUI.SetActive(false);
    }
}
