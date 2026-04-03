using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterSkinSelectUIItem : MonoBehaviour
{
    public int CharacterSkinIndex => characterSkinIndex;
    [SerializeField, ReadOnly(true)]
    private int characterSkinIndex;

    //private CharacterSkinData characterSkinData; // 캐릭터 스킨 데이터

    [SerializeField]
    private Image selectImage; // 선택 이미지

    [SerializeField]
    private Image characterSkinImage; // 캐릭터 스킨 이미지

    [SerializeField]
    private Button characterSkinButton; // 캐릭터 스킨 버튼

    [SerializeField]
    private Image lockImage; // 잠김 이미지

    [SerializeField]
    private Button purchaseButton; // 구매 버튼

    public IObservable<Unit> CharacterSkinButton => characterSkinButton.OnClickAsObservable(); // 캐릭터 스킨 버튼 대리인

    /// <summary>
    /// 캐릭터 버튼 클릭 이벤트 대리자
    /// </summary>
    public IObservable<int> GetSkinNum => getSkinNumSubject;
    private Subject<int> getSkinNumSubject = new Subject<int>(); // 캐릭터 버튼 클릭 이벤트

    /// <summary>
    /// 캐릭터 구매 버튼 클릭 이벤트 대리자
    /// </summary>
    public IObservable<int> PurchaseSkin => purchaseSkin;
    private Subject<int> purchaseSkin = new Subject<int>(); // 캐릭터 구매 이벤트

    private void Awake()
    {
        purchaseButton.onClick.AddListener(OnClickPurchaseButton);
    }

    /// <summary>
    /// 스킨 정보 초기화,
    /// 스킨 UI가 활성화 될 때 실행된다. 필요 데이터 : 스킨 스프라이트, 해당 스킨 구매유무
    /// </summary>
    private void OnEnable()
    {
        var skinList = DBManager.Instance.GetCharacterData(GameManager.Instance.CurrentCharacterID).skinList;

        //// 캐릭터 스킨 총 개수를 넘어가면 비활성화 후 리턴
        //if (skinList.Count <= characterSkinIndex)
        //{
        //    //gameObject.SetActive(false);
        //    characterSkinImage.sprite = null;
        //    DataLoad().Forget();
        //    return;
        //}

        // 캐릭터 스킨 총 개수를 넘어가면 비활성화
        characterSkinImage.sprite = skinList.Count > characterSkinIndex ? skinList[characterSkinIndex].skinSprite : null;
        DataLoad().Forget();
    }

    /// <summary>
    /// 데이터 Load 및 초기화
    /// </summary>
    /// <returns></returns>
    public async UniTask DataLoad()
    {
        var data = await SaveManager.Instance.CharacterSaveManager.GetReadOnly(GameManager.Instance.CurrentCharacterID); // DB에서 캐릭터 데이터를 가져온다

        if (data == null) // 데이터가 없다면 리턴
            return;

        bool result = data.purchaseSkinArr[characterSkinIndex]; // Index에 해당하는 스킨 구매 여부를 가져온다

        // 해당 스킨을 구매하지 않았으면
        if (!result)
        {
            lockImage.gameObject.SetActive(true); // 잠금 이미지 활성화
            purchaseButton.gameObject.SetActive(true); // 구매 버튼 활성화
        }
        // 구매했다면
        else
        {
            lockImage.gameObject.SetActive(false); // 잠금 이미지 비활성화
            purchaseButton.gameObject.SetActive(false); // 구매 버튼 비활성화
        }

        // 스프라이트 적용은 OnEnable에서 진행함
        // 버그나면 OnEnable을 없애고 DataLoad로 넘어오자
    }

    /// <summary>
    /// 버튼 클릭 시 내부적으로 해야 할 작업
    /// 해당 버튼이 Select이 된다.
    /// </summary>
    public void OnClickAction()
    {
        selectImage.gameObject.SetActive(true); // 선택 이미지 활성화

        getSkinNumSubject.OnNext(characterSkinIndex); // 스킨 Index 전달
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
        purchaseSkin.OnNext(characterSkinIndex);
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
