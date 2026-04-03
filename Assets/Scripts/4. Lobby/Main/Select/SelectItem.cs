using System;
using UniRx;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class SelectItem : MonoBehaviour
{
    private int value; // 데이터 값 > Character는 ID, Skin은 SkinIndex, Block은 ID

    [SerializeField]
    private Image selectImage; // 선택 이미지

    [SerializeField]
    private Image characterImage; // 캐릭터 이미지

    [SerializeField]
    private Button characterButton; // 캐릭터 버튼

    [SerializeField]
    private Image lockImage; // 잠김 이미지

    [SerializeField]
    private Button purchaseButton; // 구매 버튼

    public IObservable<int> OnClickItemObv => onClickItem; // 클릭 이벤트 대리인
    private Subject<int> onClickItem = new Subject<int>(); // 클릭 이벤트

    public IObservable<int> OnDeSelectItemObv => onDeSelectItem; // 선택해제 이벤트 대리인
    private Subject<int> onDeSelectItem = new Subject<int>(); // 선택해제 이벤트

    public IObservable<int> onClickPurchaseItemObv => onClickPurchaseItem; // 구매 버튼 클릭 이벤트 대리인
    private Subject<int> onClickPurchaseItem = new Subject<int>(); // 구매 버튼 클릭 이벤트

    /// <summary>
    /// 버튼 클릭 시 내부적으로 해야 할 작업
    /// 해당 버튼이 Select이 된다.
    /// </summary>
    public void OnClickAction()
    {
        selectImage.gameObject.SetActive(true); // 선택 이미지 활성화

        onClickItem.OnNext(value); // 
    }

    /// <summary>
    /// 선택 해제될 시, 해야할 작업
    /// </summary>
    public void OnDeSelect()
    {
        selectImage.gameObject.SetActive(false); // 선택 이미지 비활성화

        onDeSelectItem.OnNext(value);
    }

    /// <summary>
    /// 구매 버튼 클릭 시
    /// </summary>
    public void OnClickPurchaseButton()
    {
        onClickPurchaseItem.OnNext(value);
    }

    private void OnDisable()
    {
        OnDeSelect();
    }
}
