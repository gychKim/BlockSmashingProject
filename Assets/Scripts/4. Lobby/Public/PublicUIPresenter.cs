using UniRx;

public class PublicUIPresenter : BasePresenter<PublicUIView, PublicUIModel>
{
    public PublicUIPresenter(PublicUIView view, PublicUIModel model) : base(view, model)
    {
        this.view = view;
        this.model = model;
    }

    public override void Bind()
    {
        GameManager.Instance.CurrentGold
            .Subscribe(currentGold =>
            {
                view.CurrentGoldText.text = currentGold.ToString();
            }).AddTo(view.Disposables);

        GameManager.Instance.CurrentDia
            .Subscribe(currentDia =>
            {
                view.CurrentDiaText.text = currentDia.ToString();
            }).AddTo(view.Disposables);
    }

    public override void Destroy()
    {
        
    }
}
