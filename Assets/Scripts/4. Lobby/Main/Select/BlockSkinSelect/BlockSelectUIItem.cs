using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

/// <summary>
/// 블록 결정
/// </summary>
public class BlockSelectUIItem : MonoBehaviour
{
    [SerializeField, ReadOnly(true)]
    private int blockID; // 블록 Number

    [SerializeField]
    private Image selectImage; // 선택 이미지

    [SerializeField]
    private Image blockSkinImage; // 블록 스킨 이미지

    [SerializeField]
    private Button blockSkinButton; // 블록 스킨 버튼

    [SerializeField]
    private Image lockImage; // 잠김 이미지

    [SerializeField]
    private Button purchaseButton; // 구매 버튼

    public IObservable<Unit> BlockSkinButton => blockSkinButton.OnClickAsObservable(); // 블록 스킨 버튼 대리인

    /// <summary>
    /// 블록 버튼 클릭 이벤트 대리자
    /// </summary>
    public IObservable<int> GetBlockID => getblockIDSubject;
    private Subject<int> getblockIDSubject = new Subject<int>(); // 블록 버튼 클릭 이벤트

    /// <summary>
    /// 블록 구매 버튼 클릭 이벤트 대리자
    /// </summary>
    public IObservable<int> PurchaseBlock => purchaseBlock;
    private Subject<int> purchaseBlock = new Subject<int>(); // 블록 구매 이벤트

    private void Awake()
    {
        purchaseButton.onClick.AddListener(OnClickPurchaseButton);
    }
    private void Start()
    {
        DataLoad().Forget();
    }

    /// <summary>
    /// 데이터 Load 및 초기화
    /// </summary>
    /// <returns></returns>
    public async UniTask DataLoad()
    {
        var blockData = await SaveManager.Instance.BlockSaveManager.GetReadOnly(blockID); // 블록 데이터를 가져온다. > Json화 시켜야 한다.

        if (blockData == null) // 데이터가 없다면 비활성화 후 리턴
        {
            //// 이 부분에 들어오는 일은 없다.
            //// 각 버튼은 각자의 블록을 담당하기에, 블록 개수에 알맞게 UI아이템을 생성하므로 이 로직을 실행될 일은 없다.
            //gameObject.SetActive(false);
            return;
        }

        if (!blockData.isPurchase) // 해당 블록를 구매하지 않았으면
        {
            lockImage.gameObject.SetActive(true); // 잠금 이미지 활성화
            purchaseButton.gameObject.SetActive(true); // 구매 버튼 활성화
        }
        else // 구매했다면
        {
            lockImage.gameObject.SetActive(false); // 잠금 이미지 비활성화
            purchaseButton.gameObject.SetActive(false); // 구매 버튼 비활성화
        }

        blockSkinImage.sprite = DBManager.Instance.GetBlockData(blockID).blockSprite; // 블록 이미지 설정
    }

    /// <summary>
    /// 버튼 클릭 시 내부적으로 해야 할 작업
    /// 해당 버튼이 Select이 된다.
    /// </summary>
    public void OnClickAction()
    {
        selectImage.gameObject.SetActive(true); // 선택 이미지 활성화

        getblockIDSubject.OnNext(blockID); // 블록 ID 전달
    }

    /// <summary>
    /// 선택 해제될 시, 해야할 작업
    /// </summary>
    public void OnDeSelect()
    {
        selectImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 구매 버튼 클릭 시
    /// </summary>
    public void OnClickPurchaseButton()
    {
        // 블록 ID를 이벤트로 뿌려준다.
        purchaseBlock.OnNext(blockID);
    }

    private void OnDisable()
    {
        OnDeSelect();
    }

    private void OnDestroy()
    {
        purchaseButton.onClick.RemoveListener(OnClickPurchaseButton);
    }

    ///// <summary>
    ///// 블록 스킨 버튼 클릭 이벤트 대리자
    ///// 클릭할 시, 해당 블록 스킨 데이터를 리턴한다.
    ///// </summary>
    ///// <returns></returns>
    //public IObservable<BlockSkinData> GetSkinDataOnClick()
    //{
    //    return BlockSkinButton
    //        .Select(_ => blockSkinData);
    //    return null;
    //}
}
