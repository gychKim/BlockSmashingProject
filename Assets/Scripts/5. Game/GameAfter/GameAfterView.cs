using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameAfterView : MonoBehaviour, IUIView
{
    #region 다음 스테이지 UI
    // 다음 스테이지 UI
    public GameObject NextStageUI => nextStageUI;
    [SerializeField]
    private GameObject nextStageUI;

    // 최종 획득한 점수
    public TextMeshProUGUI FinalBlockScoreText => finalBlockScoreText;
    [SerializeField]
    private TextMeshProUGUI finalBlockScoreText;


    // 획득한 골드
    public TextMeshProUGUI GetGoldText => getGoldText;
    [SerializeField]
    private TextMeshProUGUI getGoldText;

    // 획득한 다이아
    public TextMeshProUGUI GetDiaText => getDiaText;
    [SerializeField]
    private TextMeshProUGUI getDiaText;

    // 다음 스테이지 버튼
    public Button NextStageButton => nextStageButton;
    [SerializeField]
    private Button nextStageButton;

    // 타이틀 버튼
    public Button TitleButton => titleButton;
    [SerializeField]
    private Button titleButton;

    #endregion

    #region 게임결과 UI
    // 게임 클리어 UI
    public GameObject GameResultUI => gameResultUI;
    [SerializeField]
    private GameObject gameResultUI;

    // 최종 골드
    public TextMeshProUGUI TotalGoldText => totalGoldText;
    [SerializeField]
    private TextMeshProUGUI totalGoldText;

    // 최종 다이아
    public TextMeshProUGUI TotalDiaText => totalDiaText;
    [SerializeField]
    private TextMeshProUGUI totalDiaText;

    // 최종 스테이지
    public TextMeshProUGUI FinalStageText => finalStageText;
    [SerializeField]
    private TextMeshProUGUI finalStageText;

    // 최종 획득한 점수
    public TextMeshProUGUI FinalBlockScoreText1 => finalBlockScoreText1;
    [SerializeField]
    private TextMeshProUGUI finalBlockScoreText1;

    // 재시작 버튼
    public Button RestartButton => restartButton;
    [SerializeField]
    private Button restartButton;

    // 타이틀 버튼
    public Button TitleButton1 => titleButton1;
    [SerializeField]
    private Button titleButton1;

    #endregion

    public GameObject RootObject => gameObject;

    public CompositeDisposable Disposables { get; } = new CompositeDisposable();

    /// <summary>
    /// 메인게임 시작 시 > 모든 UI를 닫는다.
    /// </summary>
    public void StartMainGame()
    {
        nextStageUI.gameObject.SetActive(false);
        gameResultUI.gameObject.SetActive(false);
        //gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Disposables.Dispose();
    }
}
