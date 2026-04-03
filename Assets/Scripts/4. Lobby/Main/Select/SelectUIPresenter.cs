using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

public class SelectUIPresenter : IPresenter<SelectUIView, SelectUIModel>
{
    // 이벤트 키
    private Guid applyCharacterEventKey, playCharacterAnimEventKey, applySkinEventKey, applyBlockEventKey, swapMainUIEventKey;

    private SelectUIView view; // 뷰
    private SelectUIModel model; // 모델

    public SelectUIPresenter(SelectUIView view, SelectUIModel model)
    {
        this.view = view;
        this.model = model;

        /***********************이벤트구독***************************************/
        applyCharacterEventKey = EventManager.Instance.Subscribe<UIEventType, int>(UIEventType.ApplyCharacter, ApplyCharacter);

        playCharacterAnimEventKey = EventManager.Instance.Subscribe(UIEventType.PlayCharacterAnim, StartCharacterAnim);

        applySkinEventKey = EventManager.Instance.Subscribe<UIEventType, int>(UIEventType.ApplySkin, ApplySkin);

        applyBlockEventKey = EventManager.Instance.Subscribe<UIEventType, int>(UIEventType.ApplyBlock, ApplyBlock);

        swapMainUIEventKey = EventManager.Instance.Subscribe(UIEventType.SwapMainUI, ResetUI);
        /***********************이벤트구독***************************************/
    }

    /// <summary>
    /// SelectUI를 리셋시킨다.
    /// </summary>
    private void ResetUI()
    {
        view.CharacterSelectObject.SetActive(false);
        view.CharacterSkinSelectObject.SetActive(false);
        view.BlockSelectObject.SetActive(false);
    }

    public void Bind()
    {
        // 비동기 데이터 Load를 진행시킨다.
        DataLoad().Forget();

        view.CharacterSelectButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.CharacterSelectObject.SetActive(true);
                view.CharacterSkinSelectObject.SetActive(false);
                view.BlockSelectObject.SetActive(false);
            }).AddTo(view.Disposables);

        view.CharacterSkinSelectButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.CharacterSelectObject.SetActive(false);
                view.CharacterSkinSelectObject.SetActive(true);
                view.BlockSelectObject.SetActive(false);
            }).AddTo(view.Disposables);

        view.BlockSelectButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.CharacterSelectObject.SetActive(false);
                view.CharacterSkinSelectObject.SetActive(false);
                view.BlockSelectObject.SetActive(true);
            }).AddTo(view.Disposables);

        view.CharacterSelectObject.SetActive(false);
        view.CharacterSkinSelectObject.SetActive(false);
        view.BlockSelectObject.SetActive(false);
    }

    /// <summary>
    /// 비동기 데이터 Load 후 데이터 적용
    /// </summary>
    /// <returns></returns>
    private async UniTask DataLoad()
    {
        // 현재 캐릭터 ID에 해당하는 캐릭터로 버튼 이미지를 전환한다.
        ApplyCharacter(GameManager.Instance.CurrentCharacterID);

        // 현재 캐릭터의 데이터를 가져온다. 
        var characterData = await SaveManager.Instance.CharacterSaveManager.GetReadOnly(GameManager.Instance.CurrentCharacterID);

        if (characterData == null)
            DebugX.Log("Non CharacterDAta");

        DebugX.Log($"CharaacterData : {characterData.characterName}, {characterData.characterID}");
        // 현재 캐릭터가 적용중인 스킨의 Index를 가져온다.
        ApplySkin(characterData.currentSkinIndex);

        // 저장된 블록 ID에 맞는 블록으로 전환한다.
        ApplyBlock(GameManager.Instance.CurrentBlockID);
    }

    /// <summary>
    /// 캐릭터 결정됐음을 전달받으면
    /// </summary>
    private async void ApplyCharacter(int characterID)
    {
        // UI가 켜져있다면 끈다.
        if (view.CharacterSelectObject.activeSelf)
            view.CharacterSelectObject.SetActive(false);

        DebugX.Log($"GetCharacterData : {DBManager.Instance.GetCharacterData(characterID).characterName}");

        view.CharacterSelectButton.image.sprite = DBManager.Instance.GetCharacterData(characterID).characterSprite; // 캐릭터 선택 버튼 이미지 변경

        PlayCharacterAnim(characterID); // 캐릭터 애니메이션 적용 및 실행

        // 스킨 Select 변경
        var characterData = await SaveManager.Instance.CharacterSaveManager.GetReadOnly(characterID);
        ApplySkin(characterData.currentSkinIndex);
    }

    /// <summary>
    /// 캐릭터 스킨을 결정됐음을 전달받으면
    /// </summary>
    private void ApplySkin(int skinIndex)
    {
        // UI가 켜져있다면 끈다.
        if (view.CharacterSkinSelectObject.activeSelf)
            view.CharacterSkinSelectObject.SetActive(false); // 캐릭터 스킨 선택 UI 비활성화

        // 스킨 적용
        ApplySkinAsync(skinIndex);
    }

    /// <summary>
    /// 스킨적용 비동기 처리
    /// </summary>
    /// <param name="skinNum"></param>
    /// <returns></returns>
    private void ApplySkinAsync(int skinNum)
    {
        if (skinNum == -1)
            return;

        var data = DBManager.Instance.GetCharacterData(GameManager.Instance.CurrentCharacterID);

        view.CharacterSelectButton.image.sprite = data.characterSprite; // 캐릭터 선택 버튼 이미지 변경

        try
        {
            view.CharacterSkinSelectButton.image.sprite = data.skinList[skinNum].skinSprite; // 캐릭터 스킨 선택 버튼 이미지 변경
        }
        catch(ArgumentOutOfRangeException)
        {
            view.CharacterSkinSelectButton.image.sprite = null;
        }

        EventManager.Instance.Publish(UIEventType.PlayCharacterAnim);
    }

    /// <summary>
    /// 블록 결정됐음을 전달받으면
    /// </summary>
    private void ApplyBlock(int blockID)
    {
        DebugX.Log($"블록 ID : {blockID}");
        // UI가 켜져있다면 끈다.
        if (view.BlockSelectObject.activeSelf)
            view.BlockSelectObject.SetActive(false); // 블록 선택 UI 비활성화

        view.BlockSelectButton.image.sprite = DBManager.Instance.GetBlockData(blockID).blockSprite; // 블록 이미지 변경
    }

    private void StartCharacterAnim()
    {
        PlayCharacterAnim().Forget();
    }


    /// <summary>
    /// 애니메이션 실행
    /// </summary>
    private async UniTask PlayCharacterAnim()
    {
        // 데이터매니저에서 현재 설정된 캐릭터 ID의 캐릭터데이터를 가져와 해당 캐릭터의 애니메이션을 실행시킨다.
        var saveData = await SaveManager.Instance.CharacterSaveManager.GetReadOnly(GameManager.Instance.CurrentCharacterID);

        var data = DBManager.Instance.GetCharacterData(GameManager.Instance.CurrentCharacterID);
        view.Animator.runtimeAnimatorController = data.uiAnimatorController;
        view.Animator.Play($"{data.characterName}_{saveData.currentSkinIndex}_Idle");
    }

    /// <summary>
    /// characterID에 해당하는 캐릭터 애니메이션 실행 > 캐릭터 선택되었을 시 호출
    /// </summary>
    /// <param name="characterID"></param>
    private async void PlayCharacterAnim(int characterID)
    {
        // 데이터매니저에서 현재 설정된 캐릭터 ID의 캐릭터데이터를 가져와 해당 캐릭터의 애니메이션을 실행시킨다.
        var saveData = await SaveManager.Instance.CharacterSaveManager.GetReadOnly(characterID);

        var data = DBManager.Instance.GetCharacterData(characterID);
        view.Animator.runtimeAnimatorController = data.uiAnimatorController;
        view.Animator.Play($"{data.characterName}_{saveData.currentSkinIndex}_Idle");
    }

    public void Disable()
    {
        
    }

    public void Destroy()
    {
        EventManager.Instance.Unsubscribe(UIEventType.ApplyCharacter, applyCharacterEventKey);

        EventManager.Instance.Unsubscribe(UIEventType.PlayCharacterAnim, playCharacterAnimEventKey);

        EventManager.Instance.Unsubscribe(UIEventType.ApplySkin, applySkinEventKey);

        EventManager.Instance.Unsubscribe(UIEventType.ApplyBlock, applyBlockEventKey);

        EventManager.Instance.Unsubscribe(UIEventType.SwapMainUI, swapMainUIEventKey);
    }
}
