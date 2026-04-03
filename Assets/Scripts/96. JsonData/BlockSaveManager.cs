using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 런타임 시, 캐릭터 데이터 수정 및 저장을 담당
/// </summary>
[System.Serializable]
public class BlockSaveManager : ISaveManager
{
    public List<BlockSaveData> BlockSaveDataList => blockSaveDataList;
    private List<BlockSaveData> blockSaveDataList = new();

    private Dictionary<int, BlockSaveData> blockSaveDataDict = new();

    [SerializeField]
    private BlockInitData blockInitData; // 최초 게임 실행 시, blockJsonData에 최초데이터를 저장하기 위해 사용하는 SO(Scriptable Object)

    public bool IsDirty => isDirty;
    private bool isDirty = false; // 변경되었는지 확인

    public bool IsLoadData => isLoadData;
    private bool isLoadData; // 데이터를 Load했는지 여부

    /// <summary>
    /// 초기화
    /// </summary>
    /// <param name="saveDataList"></param>
    public void Init()
    {
        blockSaveDataList = blockInitData.blockSaveDataList;
        blockSaveDataDict = blockInitData.blockSaveDataList.ToDictionary(x => x.blockID);
    }

    /// <summary>
    /// Json데이터에 블록 정보 저장
    /// isForce는 강제적으로 Save 시킬 때 사용, true면 반드시 저장
    /// </summary>
    /// <param name="jsonData"></param>
    public void Save(JsonData jsonData, bool isForce = false)
    {
        // 데이터 변경이 되지 않았고, isForce가 false면 리턴
        if (!isDirty && !isForce)
            return;

        jsonData.blockList = blockSaveDataList; // json데이터 변경

        isDirty = false; // 데이터 변경 여부 초기화
    }

    /// <summary>
    /// Json데이터를 받아와 데이터 갱신을 한다.
    /// </summary>
    /// <param name="jsonData"></param>
    public void Load(JsonData jsonData)
    {
        blockSaveDataList = jsonData.blockList;
        blockSaveDataDict = blockSaveDataList.ToDictionary(x => x.blockID);

        isLoadData = true;
    }

    /// <summary>
    /// 블록 저장 데이터를 원본이 아닌 복사본을 받는다.
    /// </summary>
    /// <param name="blockID"></param>
    /// <returns></returns>
    public async UniTask<BlockSaveData> GetReadOnly(int blockID)
    {
        var data = await Get(blockID);

        if (data == null)
            return null;

        // 새로 복사본을 만들어 그 데이터를 리턴한다. 
        return new BlockSaveData
        {
            blockID = blockID,
            blockName = data.blockName,
            isPurchase = data.isPurchase,
        };
    }

    /// <summary>
    /// 블록 구매 여부 변경
    /// </summary>
    public async UniTask UpdatePurchase(int blockID, bool value)
    {
        var blockSaveData = await Get(blockID); // 블록 ID에 해당하는 블록SaveData를 가져온다.

        if (blockSaveData.isPurchase == value)
            return;

        blockSaveData.isPurchase = value; // 가져온 블록의 구매 여부를 변경한다.

        isDirty = true; // 데이터 변경되었으니 true로 변경
    }

    /// <summary>
    /// 블록 ID에 해당하는 블록SaveData를 가져온다.
    /// </summary>
    /// <param name="blockID"></param>
    /// <returns></returns>
    private async UniTask<BlockSaveData> Get(int blockID)
    {
        // 데이터 Load를 하지 않았으면 대기
        while (!isLoadData)
        {
            await UniTask.Yield();
        }

        if (blockSaveDataDict.TryGetValue(blockID, out var blockSaveData))
        {
            return blockSaveData;
        }
        return null;
    }
}
