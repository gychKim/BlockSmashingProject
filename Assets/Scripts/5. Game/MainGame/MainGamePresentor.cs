using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class MainGamePresentor : BasePresenter<MainGameView, MainGameModel>
{
    // 아래 코드를 아마 사용안해도 될 것 같다.
    // 왜냐하면, Presentor과 Controller는 항상 존재하며, 최초 초기화 이후 초기화를 하지 않기 때문이다. 허나, 씬 전환이 한번이라도 발생하면, Start를 2번 실행하니, 씬 전환 작업이 추가되면, 이 코드를 부활시켜야 한다.
    // (초기화는 Model이 하는데, Controller는 그 초기화되는 Model을 관리만 하므로, Controller는 딱히 초기화 할 필요 없음)
    //private List<IDisposable> disposableList = new List<IDisposable>(); // 모델의 IObservable의 구독을 관리하는 리스트 > Presentor가 비활성화될 때 구독해제한다

    //private MainGameView view; // 뷰
    //private MainGameModel model; // 모델

    private Guid gameStartEventKey, gameReStartEventKey, gameEndEventKey;
    public MainGamePresentor(MainGameView view, MainGameModel model) : base(view,model)
    {
        // 뷰, 모델 초기화
        this.view = view;
        this.model = model;

        // 이벤트 등록
        gameStartEventKey = EventManager.Instance.Subscribe(MainGameEventType.GameStart, StartGame);
        gameReStartEventKey = EventManager.Instance.Subscribe(MainGameEventType.GameReStart, ReStartGame, null);
        gameEndEventKey = EventManager.Instance.Subscribe(MainGameEventType.GameEnd, CloseMainUI, null);
    }

    public override void Bind()
    {
        view.LeftButton
            .OnPointerUpAsObservable()
            //.OnClickAsObservable()
            .Subscribe(_ =>
            {
#if UNITY_EDITOR
                DebugX.OrangeLog("좌클릭");
                MainGameManager.Instance.stopwatch.Start();
#endif

                EventManager.Instance.Publish(MainGameEventType.LeftClick);
            }).AddTo(view.Disposables);

        view.RightButton
            .OnPointerUpAsObservable()
            //.OnClickAsObservable()
            .Subscribe(_ =>
            {
#if UNITY_EDITOR
                DebugX.OrangeLog("우클릭");
                MainGameManager.Instance.stopwatch.Start();
#endif

                EventManager.Instance.Publish(MainGameEventType.RightClick);
            }).AddTo(view.Disposables);

        view.GamePauseButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                EventManager.Instance.Publish(MainGameEventType.GamePause);
                view.GamePauseUI.SetActive(true);
            }).AddTo(view.Disposables);

        view.GameResumButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                EventManager.Instance.Publish(MainGameEventType.GameResum);
                view.GamePauseUI.SetActive(false);
            }).AddTo(view.Disposables);

        view.TitleButton2
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.GamePauseUI.SetActive(false);
                SceneManagerEX.Instance.LoadScene("LobbyScene");

            }).AddTo(view.Disposables);

        model.CurrentScore
            .Subscribe(value =>
            {
                if (value <= 0)
                {
                    view.ScoreSlider.value = 0;
                    return;
                }

                view.ScoreSlider.value = value / model.GoalScore.Value;

                float v = value / model.GoalScore.Value;
                float t = 0f;
                Color color;
                if (v < 0.5f)
                {
                    t = v / 0.5f;
                    color = new Color(t, 1f, 0f);
                }
                else
                {
                    t = (v - 0.5f) / 0.5f;
                    color = new Color(1f, 1f - t, 0f);
                }

                view.ScoreFillImage.color = color;

            }).AddTo(view.Disposables);

        model.CurrentGameTime
            .Subscribe(time =>
            {
                view.GameTimerText.text = time.ToString();
            }).AddTo(view.Disposables);

        model.CurrentGameTimeSpeed
            .Subscribe(speed =>
            {
                if (speed <= 0)
                    view.GameTimerText.color = Color.gray;
                else
                    view.GameTimerText.color = Color.red;
            }).AddTo(view.Disposables);

        model.CurrentComboTimer
            .Subscribe(value =>
            {
                if(value <= 0 && view.ComboTimerUI != null)
                {
                    view.ComboTimerUI.gameObject.SetActive(false);
                    return;
                }

                if (!view.ComboTimerUI.gameObject.activeSelf)
                    view.ComboTimerUI.gameObject.SetActive(true);

                view.ComboTimerUI.value = value;

            }).AddTo(view.Disposables);

        model.CurrentCombo
            .Subscribe(value =>
            {
                if(value <= 0 && view.CurrentComboText != null)
                {
                    view.CurrentComboText.gameObject.SetActive(false);
                    view.ComboTimerUI.gameObject.SetActive(false);
                    return;
                }

                if(!view.CurrentComboText.gameObject.activeSelf)
                    view.CurrentComboText.gameObject.SetActive(true);

                Color color;
                if (value >= 0 && value < 10)
                    color = Color.white;
                else if(value >= 10 && value < 20)
                    color = Color.red;
                else if(value >= 20  && value < 30)
                    color = Color.green;
                else if(value >= 30 && value < 40)
                    color = Color.blue;
                else if(value >= 40 && value < 50)
                    color = Color.yellow;
                else
                    color = Color.black;

                view.CurrentComboText.color = color;
                view.CurrentComboText.text = value.ToString();
            }).AddTo(view.Disposables);

        model.CurrentShieldCount
            .Subscribe(count =>
            {
                if (count <= 0 && view.CurrentShieldUI != null)
                {
                    view.CurrentShieldUI.SetActive(false);
                    return;
                }
                else
                {
                    if (!view.CurrentShieldUI.activeSelf)
                        view.CurrentShieldUI.SetActive(true);

                    view.CurrentShieldCountText.text = count.ToString();
                }
            }).AddTo(view.Disposables);

        model.CurrentFeverTimer
            .Subscribe(value =>
            {
                if (value <= 0 && view.CurrentFeverUI != null)
                {
                    view.CurrentFeverUI.SetActive(false);
                    return;
                }

                if (!view.CurrentFeverUI.activeSelf)
                    view.CurrentFeverUI.SetActive(true);

                view.CurrentFeverImage.fillAmount = value;
            }).AddTo(view.Disposables);

        model.CurrentItemSpawnChanceTimer
            .Subscribe(value =>
            {
                if (value <= 0 && view.CurrentItemChanceUI != null)
                {
                    view.CurrentItemChanceUI.SetActive(false);
                    return;
                }

                if (!view.CurrentItemChanceUI.activeSelf)
                    view.CurrentItemChanceUI.SetActive(true);

                view.CurrentItemChanceImage.fillAmount = value;
            }).AddTo(view.Disposables);

        view.GamePauseUI.SetActive(false);
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void StartGame()
    {
        view.MainGameUI.SetActive(true);
    }

    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void ReStartGame()
    {
        view.MainGameUI.SetActive(true);
    }

    /// <summary>
    /// MainUI 닫기
    /// </summary>
    private void CloseMainUI()
    {
        view.MainGameUI.SetActive(false);
    }

    /// <summary>
    /// 게임 결과 UI가 열릴 때
    /// </summary>
    public void OpenGameResultUI()
    {
        EventManager.Instance.Publish(MainGameEventType.GamePause);
    }

    public override void Destroy()
    {
        EventManager.Instance.Unsubscribe(MainGameEventType.GameStart, gameStartEventKey);
        EventManager.Instance.Unsubscribe(MainGameEventType.GameReStart, gameReStartEventKey);
        EventManager.Instance.Unsubscribe(MainGameEventType.GameEnd, gameEndEventKey);
    }
}
