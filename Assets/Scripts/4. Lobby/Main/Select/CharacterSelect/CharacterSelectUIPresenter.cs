using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterSelectUIPresenter : IPresenter<CharacterSelectUIView, CharacterSelectUIModel>
{
    private CharacterSelectUIView view;
    private CharacterSelectUIModel model; // 모델

    private CharacterSelectItemUI currentSelectItemUI; // 현재 선택된 UI > 이걸보니, ScrollView를 관리하는 스크립트가 하나 필요함을 느낀다.

    private Guid playCharacterAnimEventKey, swapMainUIEventKey, applyCharacterEventKey;

    public CharacterSelectUIPresenter(CharacterSelectUIView view, CharacterSelectUIModel model)
    {
        this.view = view;
        this.model = model;

        // 이벤트 구독
        //playCharacterAnimEventKey = EventManager.Instance.Subscribe(UIEventType.PlayCharacterAnim, StartCharacterAnim);
        swapMainUIEventKey = EventManager.Instance.Subscribe(UIEventType.SwapMainUI, ResetUI);
        //applyCharacterEventKey = EventManager.Instance.Subscribe<UIEventType, int>(UIEventType.ApplyCharacter, PlayCharacterAnim);

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

    //private void StartCharacterAnim()
    //{
    //    PlayCharacterAnim().Forget();
    //}

    public void Bind()
    {
        #region ScrollView(캐릭터 선택)
        // 시작 시 설정된 캐릭터 애니메이션 실행
        //StartCharacterAnim();

        // 결정 버튼 클릭 시
        view.ApplyButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.ApplyButton.gameObject.SetActive(false); // 결정 버튼 비활성화
                currentSelectItemUI.OnDeSelect(); // 현재 선택된 UI 선택해제 로직 실행
                currentSelectItemUI = null; // 현재 선택된 UI 초기화
                model.ApplyCharacter(); // 캐릭터 적용
            }).AddTo(view.Disposables);

        // 닫기 버튼 클릭 시
        view.CloseButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                model.CloseUI(); // 모델 Disable 실행
                view.RootObject.SetActive(false); // 현재 UI를 닫는다.

                // 캐릭터 애니메이터 적용 및 실행
                EventManager.Instance.Publish(UIEventType.PlayCharacterAnim);

            }).AddTo(view.Disposables);

        foreach (var ui in view.CharacterSelectUIList)
        {
            // 캐릭터 버튼 클릭 시 
            ui.CharacterButton
                .Subscribe(_ =>
                {
                    // 이미 선택했던 다른 UI아이템이 존재하면, OnDeSelect 호출
                    if (currentSelectItemUI != null)
                        currentSelectItemUI.OnDeSelect();

                    currentSelectItemUI = ui; // 데이터 변경 

                    ui.OnClickAction();

                    // 캐릭터 애니메이터 적용 및 실행
                    EventManager.Instance.Publish(UIEventType.PlayCharacterAnim);

                }).AddTo(view.Disposables);

            // 캐릭터 버튼 클릭 시, 해당 데이터가 지니고 있는 캐릭터 ID를 받는다.
            ui.GetCharacterID
                .Subscribe(characterID =>
                {
                    // 결정 버튼이 비활성화 상태라면, 활성화시킨다.
                    if (!view.ApplyButton.gameObject.activeSelf)
                        view.ApplyButton.gameObject.SetActive(true);

                    model.SetCharacterID(characterID); // 캐릭터 ID를 model에게 전달한다.
                }).AddTo(view.Disposables);

            // 캐릭터 구매 버튼을 눌렀을 시 호출.
            ui.PurchaseCharacter
                .Subscribe(characterID =>
                {
                    view.PurchaseUI.SetActive(true); // PurchaseUI 활성화
                    model.PurchaseReady(characterID); // 캐릭터 ID를 model에게 전달한다.
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

                // 캐릭터 구매 시도 후 구매 실패 시
                if (await model.TryPurchaseCharacter() == false)
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

            }).AddTo(view.Disposables);

        #endregion


        // 모델 데이터 갱신 이벤트
        model.UpdateDataObv
            .Subscribe(_ =>
            {
                UpdateItemData();
            }).AddTo(view.Disposables); // model도 view.Disposables이 맞나? model이 스스로 파괴하면 되는게 아닐까??
    }

    ///// <summary>
    ///// 애니메이션 실행
    ///// </summary>
    //private async UniTask PlayCharacterAnim()
    //{
    //    // 데이터매니저에서 현재 설정된 캐릭터 ID의 캐릭터데이터를 가져와 해당 캐릭터의 애니메이션을 실행시킨다.
    //    var saveData = await SaveManager.Instance.CharacterSaveManager.GetReadOnly(GameManager.Instance.CurrentCharacterID);

    //    var data = DBManager.Instance.GetCharacterData(GameManager.Instance.CurrentCharacterID);
    //    view.Animator.runtimeAnimatorController = data.uiAnimatorController;
    //    view.Animator.Play($"{data.characterName}_{saveData.currentSkinIndex}_Idle");
    //}

    ///// <summary>
    ///// characterID에 해당하는 캐릭터 애니메이션 실행 > 캐릭터 선택되었을 시 호출
    ///// </summary>
    ///// <param name="characterID"></param>
    //private async void PlayCharacterAnim(int characterID)
    //{
    //    // 데이터매니저에서 현재 설정된 캐릭터 ID의 캐릭터데이터를 가져와 해당 캐릭터의 애니메이션을 실행시킨다.
    //    var saveData = await SaveManager.Instance.CharacterSaveManager.GetReadOnly(characterID);

    //    var data = DBManager.Instance.GetCharacterData(characterID);
    //    view.Animator.runtimeAnimatorController = data.uiAnimatorController;
    //    view.Animator.Play($"{data.characterName}_{saveData.currentSkinIndex}_Idle");
    //}

    /// <summary>
    /// Item UI들 전체 데이터 갱신
    /// </summary>
    private void UpdateItemData()
    {
        foreach (var item in view.CharacterSelectUIList)
        {
            item.DataLoad().Forget();
        }
    }

    public void Destroy()
    {
        view = null;
        model = null;

        currentSelectItemUI = null;

        EventManager.Instance.Unsubscribe(UIEventType.PlayCharacterAnim, playCharacterAnimEventKey);
        EventManager.Instance.Unsubscribe(UIEventType.SwapMainUI, swapMainUIEventKey);
    }
}
