using UniRx;

public class TitlePresenter : BasePresenter<TitleView, TitleModel>
{
    private TitleSceneInit initData;
    public TitlePresenter(TitleView view, TitleModel model, TitleSceneInit initData) : base(view, model) 
    {
        this.initData = initData;
    }

    public override void Bind()
    {
        // 로딩바의 값이 1보다 크면 화면 터치하라는 텍스쳐 오브젝트 활성화
        view.LoadingBar
            .OnValueChangedAsObservable()
            .Where(value => value >= 1f)
            .Subscribe(value =>
            {
                view.LoadingBar.gameObject.SetActive(false);
                view.LoadingText.gameObject.SetActive(false);
                view.ScreenTouchButton.gameObject.SetActive(true);
            }).AddTo(view.Disposables);

        view.ScreenTouchButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                SceneManagerEX.Instance.LoadScene("LobbyScene");
            });

        // 비동기 로딩 데이터를 전부 가져와, LoadingBar에 Bind시켜준다.
        initData.LoadValue
            .Subscribe(value =>
            {
                view.LoadingBar.value = value;
            }).AddTo(view.Disposables);
    }

    public override void Destroy()
    {
        
    }
}
