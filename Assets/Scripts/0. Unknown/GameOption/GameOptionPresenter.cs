using UniRx;
using UnityEngine;

public class GameOptionPresenter : BasePresenter<GameOptionView, GameOptionModel>
{
    public GameOptionPresenter(GameOptionView view, GameOptionModel model) : base(view, model)
    {
    }

    public override void Bind()
    {
        // 옵션 버튼
        view.OptionOpenButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.OpenOptionUI();
            }).AddTo(view.Disposables);

        // 게임 종료 재확인 버튼
        view.GameExitButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.OpenExitUI();
            }).AddTo(view.Disposables);

        // 게임 종료 버튼
        view.ExitButton
           .OnClickAsObservable()
           .Subscribe(_ =>
           {
               GameManager.Instance.GameExit();
           }).AddTo(view.Disposables);

        // 게임 종료 UI 닫기 버튼
        view.ExitCloseButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.CloseExitUI();
            }).AddTo(view.Disposables);

        // 로비 버튼
        view.LobbyButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                SceneManagerEX.Instance.LoadScene("LobbyScene");
            }).AddTo(view.Disposables);

        // UI 닫기 버튼
        view.OptionCloseButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.CloseOptionUI();
            }).AddTo(view.Disposables);
    }

    public override void Destroy()
    {
        
    }
}
