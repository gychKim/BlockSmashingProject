using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIView : MonoBehaviour, IUIView
{
    public Button StageSelectButton => stageSelectButton;
    [SerializeField]
    private Button stageSelectButton; // 스테이지 선택 버튼

    public GameObject StageSelectUI => stageSelectUI;
    [SerializeField]
    private GameObject stageSelectUI; // 스테이지 선택 UI

    public Image StageImage => stageImage;
    [SerializeField]
    private Image stageImage; // 스테이지 이미지
    public TextMeshProUGUI StageNumberText => stageNumberText;
    [SerializeField]
    private TextMeshProUGUI stageNumberText; // 스테이지 텍스트

    public TextMeshProUGUI StageNameText => stageNameText;
    [SerializeField]
    private TextMeshProUGUI stageNameText; // 스테이지 이름 텍스트

    public TextMeshProUGUI StageDescriptionText => stageDescriptionText;
    [SerializeField]
    private TextMeshProUGUI stageDescriptionText; // 스테이지 설명 텍스트

    public Button LeftButton => leftButton;
    [SerializeField]
    private Button leftButton; // 좌측 버튼
    public Button RightButton => rightButton;
    [SerializeField]
    private Button rightButton; // 우측 버튼
    public Button GameStartButton => gameStartButton;
    [SerializeField]
    private Button gameStartButton; // 게임 시작버튼

    public GameObject RootObject => gameObject;

    public CompositeDisposable Disposables { get; } = new CompositeDisposable();

    /// <summary>
    /// 스테이지 선택 UI 활성/비활성
    /// </summary>
    public void ActiveStageSelectUI(bool value) => stageSelectUI.SetActive(value);

    /// <summary>
    /// 좌측 버튼 UI 활성/비활성
    /// </summary>
    /// <param name="value"></param>
    public void ActiveLeftButton(bool value) => LeftButton.gameObject.SetActive(value);

    /// <summary>
    /// 우측 버튼 UI 활성/비활성
    /// </summary>
    /// <param name="value"></param>
    public void ActiveRightButton(bool value) => RightButton.gameObject.SetActive(value);

    private void OnDestroy()
    {
        Disposables.Dispose(); // 해제
    }
}
