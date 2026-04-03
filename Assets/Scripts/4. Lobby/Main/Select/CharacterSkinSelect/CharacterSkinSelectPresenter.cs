using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

public class CharacterSkinSelectPresenter : IPresenter<CharacterSkinSelectUIView, CharacterSkinSelectModel>
{
    // 현재 선택중인 캐릭터의 스킨 데이터
    // > 지금은 1개의 스킨데이터만 있지만, 나중에는 캐릭터 데이터가 지니고 있는 List<CharacterSkinData>를 가져와야 한다.
    // > 물론, 그때는 MainCharacter이나, GameManager나 그러한 곳에서 캐릭터 데이터를 저장하고, 그 데이터에서 가져오는 게 할 것이다.

    private CharacterSkinSelectUIView view; // 뷰
    private CharacterSkinSelectModel model; // 모델

    private CharacterSkinSelectUIItem currentSelectItemUI; // 현재 선택된 UI > 이걸보니, ScrollView를 관리하는 스크립트가 하나 필요함을 느낀다.

    private Guid swapMainUIEventKey;


    public CharacterSkinSelectPresenter(CharacterSkinSelectUIView view, CharacterSkinSelectModel model)
    {
        this.view = view;
        this.model = model;

        
        swapMainUIEventKey = EventManager.Instance.Subscribe(UIEventType.SwapMainUI, ResetUI);
    }

    /// <summary>
    /// SkinSelectUI 초기화
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
        #region ScrollView(스킨 선택)

        // 결정 버튼 클릭 시
        view.ApplyButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.ApplyButton.gameObject.SetActive(false); // 결정 버튼 비활성화

                if(currentSelectItemUI != null)
                {
                    currentSelectItemUI.OnDeSelect(); // 현재 선택된 UI 선택해제 로직 실행
                    currentSelectItemUI = null; // 현재 선택된 UI 초기화
                }
                
                model.ApplySkin(); // 스킨 적용

            }).AddTo(view.Disposables);

        // 닫기 버튼 클릭 시
        view.CloseButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                model.CloseUI();
                view.RootObject.SetActive(false); // 현재 UI를 닫는다.

            }).AddTo(view.Disposables);

        foreach (var ui in view.CharacterSkinSelectUIList)
        {
            // 캐릭터 스킨 버튼 클릭 시 
            ui.CharacterSkinButton
                .Subscribe(_ =>
                {
                    // 이미 선택중 인 스킨 이 있다면, 해제
                    if (currentSelectItemUI != null)
                        currentSelectItemUI.OnDeSelect();

                    currentSelectItemUI = ui; // 선택된 UI 저장

                    ui.OnClickAction(); // 선택된 ui Click로직 실행

                    // 결정버튼이 비활성화 되어 있다면 활성화.
                    if (!view.ApplyButton.gameObject.activeSelf)
                        view.ApplyButton.gameObject.SetActive(true);

                }).AddTo(view.Disposables);

            // 캐릭터 스킨 버튼 클릭 시, 해당 데이터가 지니고 있는 캐릭터 스킨 Number를 받는다.
            ui.GetSkinNum
                .Subscribe(skinNum =>
                {
                    // 결정 버튼이 비활성화 상태라면, 활성화시킨다.
                    if (!view.ApplyButton.gameObject.activeSelf)
                        view.ApplyButton.gameObject.SetActive(true);

                    model.SetSkinNum(skinNum); // 캐릭터 스킨 데이터를 model에게 전달한다.

                    EventManager.Instance.Publish(UIEventType.PlayCharacterAnim);
                }).AddTo(view.Disposables);

            ui.PurchaseSkin
                .Subscribe(skinIndex =>
                {
                    view.PurchaseUI.SetActive(true); // PurchaseUI 활성화
                    model.PurchaseReady(skinIndex); // 스킨 Index를 model에게 전달한다.
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

                // 스킨 구매 시도 후 구매 실패 시
                if (await model.TryPurchaseSkin() == false)
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
                view.LowGoldUI.SetActive(false);  // 골드 부족 UI Close
            }).AddTo(view.Disposables);

        #endregion

        // 모델 데이터 갱신 이벤트
        model.UpdateDataObv
            .Subscribe(_ =>
            {
                UpdateItemData();
            }).AddTo(view.Disposables); // model도 view.Disposables이 맞나? model이 스스로 파괴하면 되는게 아닐까??
    }

    /// <summary>
    /// Item UI들 전체 데이터 갱신
    /// </summary>
    private void UpdateItemData()
    {
        foreach (var item in view.CharacterSkinSelectUIList)
        {
            item.DataLoad().Forget();
        }
    }

    public void Destroy()
    {
        EventManager.Instance.Unsubscribe(UIEventType.SwapMainUI, swapMainUIEventKey);
    }
}
