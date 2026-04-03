using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSkinSelectUIView : MonoBehaviour, IUIView
{
    //public GameObject CharacterSkinSelectObject => characterSkinSelectObject; // 현재 Presenter를 포함하는 UI 오브젝트
    //[SerializeField]
    //private GameObject characterSkinSelectObject; // 현재 Presenter를 포함하는 UI 오브젝트

    #region ScrollView 파트
    public List<CharacterSkinSelectUIItem> CharacterSkinSelectUIList => characterSkinSelectUIList; // 스크롤뷰의 캐릭터 아이콘(버튼) UI들
    [SerializeField]
    private List<CharacterSkinSelectUIItem> characterSkinSelectUIList; // 스크롤뷰의 캐릭터 아이콘(버튼) UI들

    public Button ApplyButton => applyButton; // 캐릭터 결정 버튼
    [SerializeField]
    private Button applyButton; // 캐릭터 결정 버튼

    public Button CloseButton => closeButton; // 닫기 버튼
    [SerializeField]
    private Button closeButton; // 닫기 버튼
    #endregion

    #region PurchaseUI 파트
    public GameObject PurchaseUI => purchaseUI; // 구매 확인 UI
    [SerializeField]
    private GameObject purchaseUI;

    public Button AcceptButton => acceptButton; // 수락 버튼
    [SerializeField]
    private Button acceptButton;

    public Button RejectButton => rejectButton; // 거절 버튼
    [SerializeField]
    private Button rejectButton;

    #endregion

    #region LowGoldUI 파트
    public GameObject LowGoldUI => lowGoldUI; // 돈 부족 경고 UI
    [SerializeField]
    private GameObject lowGoldUI;

    public Button ConfirmButton => confirmButton; // 확인 버튼
    [SerializeField]
    private Button confirmButton;
    #endregion

    public GameObject RootObject => gameObject; // 최상위 UI
    public CompositeDisposable Disposables { get; } = new CompositeDisposable();
    private void OnDestroy()
    {
        Disposables.Dispose(); // 해제
    }

}
