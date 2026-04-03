using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TitleView : MonoBehaviour, IUIView
{
    public Slider LoadingBar => loadingBar;
    [SerializeField]
    private Slider loadingBar;  // 로딩 바

    public TextMeshProUGUI LoadingText => loadingText;
    [SerializeField]
    private TextMeshProUGUI loadingText;  // 로딩 텍스트

    public Button ScreenTouchButton => screenTouchButton;
    [SerializeField]
    private Button screenTouchButton;

    //public TextMeshProUGUI FinishText => finishText;
    //[SerializeField]
    //private TextMeshProUGUI finishText; // 화면터치 텍스트

    public GameObject RootObject => gameObject;

    public CompositeDisposable Disposables { get; } = new CompositeDisposable();

}
