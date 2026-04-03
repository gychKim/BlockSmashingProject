using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameOptionView : BaseView
{
    public GameObject OptionUI => optionUI; // 옵션 UI
    [SerializeField]
    private GameObject optionUI; // 옵션 UI

    public Button OptionOpenButton => optionOpenButton; // 옵션 열기 버튼
    [SerializeField]
    private Button optionOpenButton; // 옵션 열기 버튼

    public Button OptionCloseButton => optionCloseButton; // 옵션 닫기 버튼
    [SerializeField]
    private Button optionCloseButton; // 옵션 닫기 버튼

    public Button GameExitButton => gameExitButton; // 게임종료 열기 버튼
    [SerializeField]
    private Button gameExitButton; // 게임종료 열기 버튼

    public GameObject ExitUI => exitUI; // 게임 종료 UI
    [SerializeField]
    private GameObject exitUI; // 게임 종료 UI

    public Button ExitButton => exitButton; // 게임 종료 버튼
    [SerializeField]
    private Button exitButton; // 게임 종료 버튼

    public Button ExitCloseButton => exitCloseButton; // 게임 종료 닫기 버튼
    [SerializeField]
    private Button exitCloseButton; // 게임 종료 닫기 버튼

    /// <summary>
    /// 로비 버튼
    /// </summary>
    public Button LobbyButton => lobbyButton;
    [SerializeField]
    private Button lobbyButton;

    /// <summary>
    /// 옵션 UI 활성화
    /// </summary>
    public void OpenOptionUI()
    {
        optionUI.SetActive(true);

        gameExitButton.gameObject.SetActive(MainGameManager.Instance == null);
        lobbyButton.gameObject.SetActive(MainGameManager.Instance != null);
    }

    /// <summary>
    /// 옵션 UI 비활성화
    /// </summary>
    public void CloseOptionUI()
    {
        gameExitButton.gameObject.SetActive(false);
        lobbyButton.gameObject.SetActive(false);

        CloseExitUI();

        optionUI.SetActive(false);
    }

    /// <summary>
    /// 게임 종료 확인 UI 활성화
    /// </summary>
    public void OpenExitUI()
    {
        exitUI.gameObject.SetActive(true);
    }

    /// <summary>
    /// 게임 종료 확인 UI 비활성화
    /// </summary>
    public void CloseExitUI()
    {
        exitUI.gameObject.SetActive(false);
    }
}
