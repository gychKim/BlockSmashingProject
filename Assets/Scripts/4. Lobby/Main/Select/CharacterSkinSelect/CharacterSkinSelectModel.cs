using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

public class CharacterSkinSelectModel : IModel
{
    public int CurrentSkinNum => currentSkinNum;
    [SerializeField, ReadOnly(true)]
    private int currentSkinNum; // 현재 모델(버튼UI)가 적용하고 있는 캐릭터의 스킨 Number

    private int tempSkinNum;

    [SerializeField, ReadOnly]
    private int purchaseSkinIndex; // 구매 대기 스킨 Index

    public IObservable<Unit> UpdateDataObv => updateDataObv;
    private Subject<Unit> updateDataObv = new Subject<Unit>(); // 스킨 데이터 갱신 이벤트 > 스킨 구매 하면 데이터 갱신 필요하므로, view에게 UI Update 필요하다는 요청

    // Start에서 실행
    public CharacterSkinSelectModel()
    {
        
    }

    public void Start()
    {
        DataLoad().Forget();
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

        // 캐릭터 ID에 해당하는 캐릭터의 데이터를 가져온다.
        var characterData = await SaveManager.Instance.CharacterSaveManager.GetReadOnly(currentCharacterID);

        // 선택된 캐릭터의 SkinIndex를 가져온다.
        int currentCharacterSkinIndex = characterData.currentSkinIndex;

        // SkinIndex 설정
        currentSkinNum = currentCharacterSkinIndex;

        // 최초 캐릭터 스킨 Number의 스킨을 적용시킨다. > 이 작업을 SelectUIPresenter에게 전달하기 위해 사용했는데, 그냥 SelectUIPresenter가 Bind할 때 스킨적용로직을 실행하도록 변경
        // 왜? Start해서 데이터 최초 적용시킬 때는 이벤트를 사용하는 게 불안정해서
        //EventManager.Instance.Publish(UIEventType.SelectSkin, currentSkinNum); 

        tempSkinNum = -1;
        purchaseSkinIndex = -1;
    }

    /// <summary>
    /// 현재 선택된 캐릭터 스킨 데이터 갱신
    /// </summary>
    /// <param name="id"></param>
    public void SetSkinNum(int num)
    {
        tempSkinNum = num;

        //EventManager.Instance.Publish(UIEventType.SelectSkin, num); // 캐릭터 스킨이 클릭되어, 클릭된 캐릭터의 ID를 뿌린다.
    }

    /// <summary>
    /// 스킨 결정
    /// </summary>
    public void ApplySkin()
    {
        currentSkinNum = tempSkinNum; // 스킨 ID 설정

        // 현재 캐릭터에 스킨을 적용시킨다.
        EventManager.Instance.Publish(UIEventType.ApplySkin, currentSkinNum); // 캐릭터 스킨이 결정 됐음을 알리는 역할

        tempSkinNum = -1;
    }

    /// <summary>
    /// 스킨 구매 대기 > 스킨 구매 버튼을 누른 스킨의 Index를 임시저장한 상태
    /// </summary>
    /// <param name="characterID"></param>
    public void PurchaseReady(int skinIndex)
    {
        purchaseSkinIndex = skinIndex;
    }

    /// <summary>
    /// 스킨 구매 시도
    /// 구매 성공 시 True, 실패 시 false 리턴
    /// </summary>
    public async UniTask<bool> TryPurchaseSkin()
    {
        var characterData = DBManager.Instance.GetCharacterData(GameManager.Instance.CurrentCharacterID); // 구매 대기 캐릭터 ID에 해당하는 캐릭터 데이터를 가져온다.

        // 스킨을 구입할 수 있는 금액이 된다면
        if (GameManager.Instance.CurrentGold.Value >= characterData.skinList[purchaseSkinIndex].price)
        {
            GameManager.Instance.RemoveGold(characterData.skinList[purchaseSkinIndex].price); // 골드 차감

            // 스킨 구매여부 데이터 갱신
            await SaveManager.Instance.CharacterSaveManager.UpdateSkinPurchase(GameManager.Instance.CurrentCharacterID, purchaseSkinIndex, true);

            // 구매했다고 이벤트 전달
            updateDataObv.OnNext(Unit.Default);

            return true;
        }

        // 금액이 부족하다면
        return false;
    }

    /// <summary>
    /// UI 비활성화 시 호출
    /// </summary>
    public void CloseUI()
    {
        //// 캐릭터 스킨을 결정하지 않았으면, 이전 캐릭터 스킨 Number를 뿌린다.
        //if (tempSkinNum != -1)
        //{
        //    EventManager.Instance.Publish(UIEventType.ReturnSkin, currentSkinNum);
        //}

        // 값 초기화
        tempSkinNum = -1;
        purchaseSkinIndex = -1;
    }

    public void Destroy()
    {
        
    }
}
