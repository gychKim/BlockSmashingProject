using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

public class CharacterSelectUIModel : IModel
{
    private int currentCharacterID; // 현재 선택된 캐릭터의 ID

    [SerializeField, ReadOnly]
    private int tempCharacterID; // 임시 캐릭터 ID

    [SerializeField, ReadOnly]
    private int purchaseCharacterID; // 구매 대기 캐릭터 ID

    public IObservable<Unit> UpdateDataObv => updateDataObv;
    private Subject<Unit> updateDataObv = new Subject<Unit>(); // 캐릭터 데이터 갱신 이벤트 > 캐릭터 구매 하면 캐릭터 데이터 갱신 필요하므로, view에게 UI Update 필요하다는 요청

    public CharacterSelectUIModel()
    {
        
    }

    public void Start()
    {
        DataLoad().Forget();

        //EventManager.Instance.Publish(UIEventType.SelectCharacter, currentCharacterID); // 최초 캐릭터를 적용시킨다.
        //EventManager.Instance.Publish(UIEventType.PlayCharacterAnim); // 애니메이션 적용
    }

    /// <summary>
    /// 데이터 로드 및 초기화
    /// </summary>
    /// <returns></returns>
    private async UniTask DataLoad()
    {
        // 현재 데이터매니저에서 게임 상태 데이터를 가져온다.
        var gameStateData = await SaveManager.Instance.GameStateSaveManager.GetReadOnly();

        // 게임 상태 데이터의 현재 선택된 캐릭터 데이터의 ID 가져온다.
        int currentCharacterID = gameStateData.currentCharacterID;

        // 캐릭터 ID 적용
        this.currentCharacterID = currentCharacterID;

        // 임시ID 초기화
        tempCharacterID = -1;
        purchaseCharacterID = -1;
    }

    /// <summary>
    /// 현재 선택된 캐릭터의 ID 갱신 후 UI 변경
    /// </summary>
    /// <param name="id"></param>
    public void SetCharacterID(int id)
    {
        tempCharacterID = id; // id등록

        // 캐릭터 ID 전달
        //EventManager.Instance.Publish(UIEventType.SelectCharacter, id);
    }

    /// <summary>
    /// 캐릭터 결정
    /// </summary>
    public void ApplyCharacter()
    {
        currentCharacterID = tempCharacterID; // 캐릭터 ID 설정

        EventManager.Instance.Publish(UIEventType.ApplyCharacter, currentCharacterID); // 캐릭터 결정됐음을 알림

        tempCharacterID = -1; // 값 초기화
    }

    /// <summary>
    /// 캐릭터 구매 대기 > 캐릭터 구매 버튼을 누른 캐릭터의 ID를 임시저장한 상태
    /// </summary>
    /// <param name="characterID"></param>
    public void PurchaseReady(int characterID)
    {
        purchaseCharacterID = characterID;
    }

    /// <summary>
    /// 캐릭터 구매 시도
    /// 구매 성공 시 True, 실패 시 false 리턴
    /// </summary>
    public async UniTask<bool> TryPurchaseCharacter()
    {
        var characterData = DBManager.Instance.GetCharacterData(purchaseCharacterID); // 구매 대기 캐릭터 ID에 해당하는 캐릭터 데이터를 가져온다.

        // 캐릭터를 구입할 수 있는 금액이 된다면
        if(GameManager.Instance.CurrentGold.Value >= characterData.price)
        {
            GameManager.Instance.RemoveGold(characterData.price); // 골드 차감
            await SaveManager.Instance.CharacterSaveManager.UpdatePurchase(purchaseCharacterID, true); // 캐릭터 구매여부 데이터 갱신

            updateDataObv.OnNext(Unit.Default);
            
            return true;
        }

        // 금액이 부족하다면
        return false;
    }

    /// <summary>
    /// UI비활성화 시 호출
    /// </summary>
    public void CloseUI()
    {
        // 캐릭터를 결정하지 않았으면, 이전 캐릭터 ID를 뿌린다.
        //if(tempCharacterID != -1)
        //{
        //    EventManager.Instance.Publish(UIEventType.ReturnCharacter, currentCharacterID);
        //}

        // 값 초기화
        tempCharacterID = -1;
        purchaseCharacterID = -1;
    }

    /// <summary>
    /// Model파괴 시 호출
    /// </summary>
    public void Destroy()
    {
        
    }
}
