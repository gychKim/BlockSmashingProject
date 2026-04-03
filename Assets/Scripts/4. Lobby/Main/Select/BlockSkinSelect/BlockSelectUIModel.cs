using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

public class BlockSelectUIModel : IModel
{
    private int currentBlockID;

    private int tempBlockID; // 임시 블록 ID

    [SerializeField, ReadOnly]
    private int purchaseBlockID; // 구매 대기 블록 ID

    public IObservable<Unit> UpdateDataObv => updateDataObv;
    private Subject<Unit> updateDataObv = new Subject<Unit>(); // 블록 데이터 갱신 이벤트 > 블록 구매 하면 블록 데이터 갱신 필요하므로, view에게 UI Update 필요하다는 요청

    public BlockSelectUIModel()
    {
        currentBlockID = 1000; // 블록 ID 임시 초기화

        tempBlockID = -1;
    }

    public void Start()
    {
        DataLoad().Forget();
    }

    /// <summary>
    /// 데이터 로드 및 초기화
    /// </summary>
    /// <returns></returns>
    private  async UniTask DataLoad()
    {
        // 현재 데이터매니저에서 게임 상태 데이터를 가져온다.
        var gameStateData = await SaveManager.Instance.GameStateSaveManager.GetReadOnly();

        // 게임 상태 데이터의 현재 선택된 블록 데이터의 ID 가져온다.
        int currentBlockID = gameStateData.currentBlockID;

        // 블록 ID 적용
        this.currentBlockID = currentBlockID;

        // 임시ID 초기화
        tempBlockID = -1;
        purchaseBlockID = -1;
    }

    /// <summary>
    /// 현재 선택된 블록 스킨 데이터 갱신
    /// </summary>
    /// <param name="skinData"></param>
    public void SetBlockID(int blockID)
    {
        tempBlockID = blockID; // id등록

        //EventManager.Instance.Publish(UIEventType.SelectBlock, blockID); // 블록 ID 전달
    }

    /// <summary>
    /// 스킨 결정
    /// </summary>
    public void ApplyBlock()
    {
        currentBlockID = tempBlockID; // 블록 ID 설정

        // 현재 블록을 적용시킨다.
        EventManager.Instance.Publish(UIEventType.ApplyBlock, currentBlockID); // 이벤트 호출 > 블록이 결정되었음을 알림

        tempBlockID = -1;

        //DebugX.Log(currentSkinData.skinSprite.name);
    }

    /// <summary>
    /// 블록 구매 대기 > 블록 구매 버튼을 누른 블록의 ID를 임시저장한 상태
    /// </summary>
    /// <param name="characterID"></param>
    public void PurchaseReady(int blockID)
    {
        purchaseBlockID = blockID;
    }

    // <summary>
    /// 블록 구매 시도
    /// 구매 성공 시 True, 실패 시 false 리턴
    /// </summary>
    public async UniTask<bool> TryPurchaseBlock()
    {
        var blockData = DBManager.Instance.GetBlockData(purchaseBlockID); // 구매 대기 블록 ID에 해당하는 블록 데이터를 가져온다.

        // 블록를 구입할 수 있는 금액이 된다면
        if (GameManager.Instance.CurrentGold.Value >= blockData.price)
        {
            GameManager.Instance.RemoveGold(blockData.price); // 골드 차감
            await SaveManager.Instance.BlockSaveManager.UpdatePurchase(purchaseBlockID, true); // 블록 구매여부 데이터 갱신

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
        // 블록을 결정하지 않았으면, 이전 블록 ID를 뿌린다.
        //if (tempBlockID != -1)
        //{
        //    EventManager.Instance.Publish(UIEventType.ReturnBlock, currentBlockID);
        //}

        tempBlockID = -1; // 값 초기화
    }

    public void Destroy()
    {
        
    }
}
