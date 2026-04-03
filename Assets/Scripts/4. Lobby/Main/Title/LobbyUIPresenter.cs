using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIPresenter : IPresenter<LobbyUIView, LobbyUIModel>
{
    private LobbyUIView view;
    private LobbyUIModel model;

    private Guid swapMainUIEventKey;

    public LobbyUIPresenter(LobbyUIView view, LobbyUIModel model)
    {
        this.view = view;
        this.model = model;

        swapMainUIEventKey = EventManager.Instance.Subscribe(UIEventType.SwapMainUI, ResetUI);
    }

    public void Bind()
    {
        // 스테이지 선택 UI Open버튼
        view.StageSelectButton
            .OnClickAsObservable()
            .Subscribe((_) =>
            {
                view.ActiveStageSelectUI(true); // 스테이지 선택 UI 활성화
            }).AddTo(view.Disposables);

        // 좌측 버튼(이전 스테이지)
        view.LeftButton
            .OnClickAsObservable()
            .Subscribe((_) =>
            {
                model.PrevStage();
            }).AddTo(view.Disposables);

        // 우측 버튼(다음 스테이지)
        view.RightButton
            .OnClickAsObservable()
            .Subscribe((_) =>
            {
                model.NextStage();
            }).AddTo(view.Disposables);

        // 게임 시작 버튼
        view.GameStartButton
            .OnClickAsObservable()
            .Subscribe((_) =>
            {
                model.GameStart();
            }).AddTo(view.Disposables);

        // 현재 스테이지
        model.CurrentStage
            .Subscribe(value =>
            {
                view.StageNumberText.text = value.ToString();

                if(value <= 1)
                {
                    view.ActiveLeftButton(false);
                    view.ActiveRightButton(true);
                }
                else if(value >= 5)
                {
                    view.ActiveLeftButton(true);
                    view.ActiveRightButton(false);
                }
                else
                {
                    view.ActiveLeftButton(true);
                    view.ActiveRightButton(true);
                }
            }).AddTo(view.Disposables);

        // 스테이지 이름
        model.StageName
            .Subscribe(value =>
            {
                view.StageNameText.text = value.ToString();
            }).AddTo(view.Disposables);

        // 스테이지 설명
        model.StageDescription
            .Subscribe(value =>
            {
                view.StageDescriptionText.text = value.ToString();
            }).AddTo(view.Disposables);

        // 스테이지 이미지
        model.StageImage
            .Subscribe(sprite =>
            {
                view.StageImage.sprite = sprite;
            }).AddTo(view.Disposables);
    }

    /// <summary>
    /// SelectUI를 리셋시킨다.
    /// </summary>
    private void ResetUI()
    {
        model.Reset();
        view.ActiveStageSelectUI(false);
    }

    public void Destroy()
    {
        EventManager.Instance.Unsubscribe(UIEventType.SwapMainUI, swapMainUIEventKey);
    }
}
