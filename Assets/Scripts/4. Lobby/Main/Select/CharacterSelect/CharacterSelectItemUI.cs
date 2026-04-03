using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectItemUI : MonoBehaviour
{
    [SerializeField, ReadOnly(true)]
    private int characterID; // 해당 아이템이 연관하는 캐릭터의 ID

    //[SerializeField]
    //private CharacterShopData characterShopData; // 캐릭터 상점 데이터 > 버튼마다 미리 할당시켜준다.

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

    //[SerializeField]
    //private Image deactiveImage; // 버튼 비활성화 이미지 > 구매하지 않았다는 것을 알려주는 역할

    public IObservable<Unit> CharacterButton => characterButton.OnClickAsObservable(); // 캐릭터 버튼 대리인

    /// <summary>
    /// 캐릭터 버튼 클릭 이벤트 대리자
    /// </summary>
    public IObservable<int> GetCharacterID => getCharacterIDSubject;
    private Subject<int> getCharacterIDSubject = new Subject<int>(); // 캐릭터 버튼 클릭 이벤트

    /// <summary>
    /// 캐릭터 구매 버튼 클릭 이벤트 대리자
    /// </summary>
    public IObservable<int> PurchaseCharacter => purchaseCharacter;
    private Subject<int> purchaseCharacter = new Subject<int>(); // 캐릭터 구매 이벤트

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
        var data = await SaveManager.Instance.CharacterSaveManager.GetReadOnly(characterID); // DB에서 캐릭터 데이터를 가져온다

        if (data == null) // 데이터가 없다면 리턴
            return;

        if (!data.isPurchase) // 해당 캐릭터를 구매하지 않았으면
        {
            lockImage.gameObject.SetActive(true); // 잠금 이미지 활성화
            purchaseButton.gameObject.SetActive(true); // 구매 버튼 활성화
        }
        else // 구매했다면
        {
            lockImage.gameObject.SetActive(false); // 잠금 이미지 비활성화
            purchaseButton.gameObject.SetActive(false); // 구매 버튼 비활성화
        }

        characterImage.sprite = DBManager.Instance.GetCharacterData(characterID).characterUISprite; // 캐릭터 이미지 설정
    }

    /// <summary>
    /// 버튼 클릭 시 내부적으로 해야 할 작업
    /// 해당 버튼이 Select이 된다.

            /// </summary>
    public void OnClickAction()
    {
        selectImage.gameObject.SetActive(true); // 선택 이미지 활성화

        getCharacterIDSubject.OnNext(characterID); // 캐릭터 ID 전달
    }

    /// <summary>
    /// 선택 해제될 시, 해야할 작업
    /// </summary>
    public void OnDeSelect()
    {
        selectImage.gameObject.SetActive(false); // 선택 이미지 비활성화
    }

    /// <summary>
    /// 구매 버튼 클릭 시
    /// </summary>
    public void OnClickPurchaseButton()
    {
        // 캐릭터 ID를 이벤트로 뿌려준다.
        purchaseCharacter.OnNext(characterID);
    }

    private void OnDisable()
    {
        OnDeSelect();
    }
    private void OnDestroy()
    {
        purchaseButton.onClick.RemoveListener(OnClickPurchaseButton);
    }
}
