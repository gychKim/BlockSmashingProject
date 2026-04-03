using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BlockSelectUIPresenter : IPresenter<BlockSelectUIView, BlockSelectUIModel>
{
    private BlockSelectUIView view; // 뷰
    private BlockSelectUIModel model; // 모델

    private BlockSelectUIItem currentSelectItemUI; // 현재 선택된 UI > 이걸보니, ScrollView를 관리하는 스크립트가 하나 필요함을 느낀다.

    private Guid swapMainUIEventKey;
    public BlockSelectUIPresenter(BlockSelectUIView view, BlockSelectUIModel model)
    {
        this.view = view;
        this.model = model;

        swapMainUIEventKey = EventManager.Instance.Subscribe(UIEventType.SwapMainUI, ResetUI);
    }

    /// <summary>
    /// BlockSelect의 초기화
    /// </summary>
    private void ResetUI()
    {
        view.ApplyButton.gameObject.SetActive(false); // 결정 버튼 비활성화

        // 이미 선택중 인 버튼 이 있다면, 해제
        if (currentSelectItemUI != null)
            currentSelectItemUI.OnDeSelect();

        currentSelectItemUI = null; // 현재 선택된 UI 초기화
        model.CloseUI(); // 모델 Disable 실행
        view.RootObject.SetActive(false); // 현재 UI를 닫는다.
    }

    public void Bind()
    {
        #region ScrollView(블록 선택)
        // 결정 버튼 클릭 시
        view.ApplyButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.ApplyButton.gameObject.SetActive(false); // 결정 버튼 비활성화
                currentSelectItemUI.OnDeSelect(); // 현재 선택된 UI 선택해제 로직 실행
                currentSelectItemUI = null; // 현재 선택된 UI 초기화
                model.ApplyBlock(); // 블록 적용
            }).AddTo(view.Disposables);

        // 닫기 버튼 클릭 시
        view.CloseButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                model.CloseUI(); // 모델 Disable 실행
                view.RootObject.SetActive(false); // 현재 UI를 닫는다.
            }).AddTo(view.Disposables);

        foreach (var ui in view.BlockSelectUIList)
        {
            // 블록 버튼 클릭 시 
            ui.BlockSkinButton
                .Subscribe(_ =>
                {
                    // 이미 선택중 인 버튼 이 있다면, 해제
                    if (currentSelectItemUI != null)
                        currentSelectItemUI.OnDeSelect();

                    currentSelectItemUI = ui;

                    ui.OnClickAction();
                }).AddTo(view.Disposables);

            // 블록 버튼 클릭 시, 해당 데이터가 지니고 있는 블록 ID를 받는다.
            ui.GetBlockID
                .Subscribe(blockID =>
                {
                    // 결정 버튼이 비활성화 상태라면, 활성화시킨다.
                    if (!view.ApplyButton.gameObject.activeSelf)
                        view.ApplyButton.gameObject.SetActive(true);

                    model.SetBlockID(blockID); // 블록 데이터를 model에게 전달한다.
                }).AddTo(view.Disposables);

            // 블록 구매 버튼을 눌렀을 시 호출.
            ui.PurchaseBlock
                .Subscribe(blockID =>
                {
                    view.PurchaseUI.SetActive(true); // PurchaseUI 활성화
                    model.PurchaseReady(blockID); // 블록 ID를 model에게 전달한다.
                }).AddTo(view.Disposables);
        }

        #endregion

        #region PurchaseUI 파트

        // 구매 UI 수락 버튼
        view.AcceptButton
            .OnClickAsObservable()
            .Subscribe(async _ =>
            {
                view.PurchaseUI.SetActive(false); // 구매 UI Close > 성공했든 실패했든 반드시 닫는다.

                // 블록 구매 시도 후 구매 실패 시
                if (await model.TryPurchaseBlock() == false)
                {
                    view.LowGoldUI.SetActive(true); // 돈 부족 UI 활성화
                }
            }).AddTo(view.Disposables);

        // 구매 UI 거절 버튼
        view.RejectButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.PurchaseUI.SetActive(false); // 구매 UI Close
            }).AddTo(view.Disposables);

        #endregion
        #region LowGoldUI 파트

        // 돈 부족 UI 확인 버튼
        view.ConfirmButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.LowGoldUI.SetActive(false); // 골드 부족 UI Close
            }).AddTo(view.Disposables);

        #endregion

        // 모델 데이터 갱신 이벤트
        model.UpdateDataObv
            .Subscribe(_ =>
            {
                UpdateItemData();
            }).AddTo(view.Disposables);
    }

    /// <summary>
    /// Item UI들 전체 데이터 갱신
    /// </summary>
    private void UpdateItemData()
    {
        foreach (var item in view.BlockSelectUIList)
        {
            item.DataLoad().Forget();
        }
    }

    public void Destroy()
    {
        currentSelectItemUI = null;
        EventManager.Instance.Unsubscribe(UIEventType.SwapMainUI, swapMainUIEventKey);
    }
}
