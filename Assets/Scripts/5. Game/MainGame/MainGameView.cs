using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MainGameView : BaseView, IUIView
{
    public GameObject MainGameUI => mainGameUI;
    [SerializeField]
    private GameObject mainGameUI;
    // 좌측버튼
    public Button LeftButton => leftButton;
    [SerializeField]
    private Button leftButton;

    // 우측버튼
    public Button RightButton => rightButton;
    [SerializeField]
    private Button rightButton;

    //#region 다음 스테이지 UI
    //// 다음 스테이지 UI
    //public GameObject NextStageUI => nextStageUI;
    //[SerializeField]
    //private GameObject nextStageUI;

    //// 최종 획득한 점수
    //public TextMeshProUGUI FinalBlockScoreText => finalBlockScoreText;
    //[SerializeField]
    //private TextMeshProUGUI finalBlockScoreText;

    //// 다음 스테이지 버튼
    //public Button NextStageButton => nextStageButton;
    //[SerializeField]
    //private Button nextStageButton;

    //// 타이틀 버튼
    //public Button TitleButton => titleButton;
    //[SerializeField]
    //private Button titleButton;

    //#endregion

    //#region 게임결과 UI
    //// 게임 클리어 UI
    //public GameObject GameResultUI => gameResultUI;
    //[SerializeField]
    //private GameObject gameResultUI;

    //// 최종 스테이지
    //public TextMeshProUGUI FinalStageText => finalStageText;
    //[SerializeField]
    //private TextMeshProUGUI finalStageText;

    //// 최종 획득한 점수
    //public TextMeshProUGUI FinalBlockScoreText1 => finalBlockScoreText1;
    //[SerializeField]
    //private TextMeshProUGUI finalBlockScoreText1;

    //// 재시작 버튼
    //public Button RestartButton => restartButton;
    //[SerializeField]
    //private Button restartButton;

    //// 타이틀 버튼
    //public Button TitleButton1 => titleButton1;
    //[SerializeField]
    //private Button titleButton1;

    //#endregion

    // 현재 스테이지 Text
    public TextMeshProUGUI CurrentStageText => currentStageText;
    [SerializeField]
    private TextMeshProUGUI currentStageText;

    // 게임 시간 Text
    public TextMeshProUGUI GameTimerText => gameTimerText;
    [SerializeField]
    private TextMeshProUGUI gameTimerText;

    // 목표 점수 게이지(슬라이더)
    public Slider ScoreSlider => scoreSlider;
    [SerializeField]
    private Slider scoreSlider;

    // 목표 점수 게이지 이미지
    public Image ScoreFillImage => scoreFillImage;
    [SerializeField]
    private Image scoreFillImage;

    // 250610 게이지로 교체
    //// 목표 점수 Text
    //public TextMeshProUGUI GoalScoreText => goalScoreText;
    //[SerializeField]
    //private TextMeshProUGUI goalScoreText;

    //// 현재 총 획득 점수 Text
    //public TextMeshProUGUI CurrentScoreText => currentScoreText;
    //[SerializeField]
    //private TextMeshProUGUI currentScoreText;

    // 콤보 타이머 UI(Slider) 
    public Slider ComboTimerUI => comboTimerUI;
    [SerializeField]
    private Slider comboTimerUI;

    // 현재 콤보 Text
    public TextMeshProUGUI CurrentComboText => currentComboText;
    [SerializeField]
    private TextMeshProUGUI currentComboText;

    // 현재 블록 당 점수 text
    //public TextMeshProUGUI CurrentBlockScoreText => currentBlockScoreText;
    //[SerializeField]
    //private TextMeshProUGUI currentBlockScoreText;

    //// 점수배율 UI 오브젝트
    //public GameObject CurrentScoreMultiplierTimerUI => currentScoreMultiplierTimerUI;
    //[SerializeField]
    //private GameObject currentScoreMultiplierTimerUI;

    //// 점수 배율 효과 남은시간
    //public TextMeshProUGUI CurrentScoreMultiplierTimerText => currentScoreMultiplierTimerText;
    //[SerializeField]
    //private TextMeshProUGUI currentScoreMultiplierTimerText;

    // 실드 UI 오브젝트
    public GameObject CurrentShieldUI => currentShieldUI;
    [SerializeField]
    private GameObject currentShieldUI;

    // 실드 UI 이미지
    public TextMeshProUGUI CurrentShieldCountText => currentShieldCountText;
    [SerializeField]
    private TextMeshProUGUI currentShieldCountText;

    // 피버 UI 오브젝트
    public GameObject CurrentFeverUI => currentFeverUI;
    [SerializeField]
    private GameObject currentFeverUI;

    // 피버 효과 남은시간
    public Image CurrentFeverImage => currentFeverImage;
    [SerializeField]
    private Image currentFeverImage;

    // 아이템 획득확률 UI 오브젝트
    public GameObject CurrentItemChanceUI => currentItemChanceUI;
    [SerializeField]
    private GameObject currentItemChanceUI;

    // 아이템 획득확률 남은시간
    public Image CurrentItemChanceImage => currentItemChanceImage;
    [SerializeField]
    private Image currentItemChanceImage;

    // 게임 정지 버튼
    public Button GamePauseButton => gamePauseButton;
    [SerializeField]
    private Button gamePauseButton;

    // 게임 정지 표시 UI
    public GameObject GamePauseUI => gamePauseUI;
    [SerializeField]
    private GameObject gamePauseUI;

    // 게임 재개 버튼
    public Button GameResumButton => gameResumButton;
    [SerializeField]
    private Button gameResumButton;

    // 타이틀 버튼
    public Button TitleButton2 => titleButton2;
    [SerializeField]
    private Button titleButton2;

    private void OnDestroy()
    {
        Disposables.Dispose(); // 해제
    }
}
