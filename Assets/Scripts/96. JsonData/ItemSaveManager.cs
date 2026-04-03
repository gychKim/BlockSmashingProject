using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ItemSaveData
{
    public int itemID; // 아이템 ID
    public int itemCount; // 아이템 개수
}

/// <summary>
/// 런타임 시, 아이템 데이터 수정 및 저장, 로드를 담당
/// </summary>
[System.Serializable]
public class ItemSaveManager : ISaveManager
{
    public List<ItemSaveData> ItemSaveDataList => itemSaveDataList;
    private List<ItemSaveData> itemSaveDataList = new();

    private Dictionary<int, ItemSaveData> itemSaveDataDict = new();

    [SerializeField]
    private ItemInitData itemInitData; // 최초 게임 실행 시, itemJsonData에 최초데이터를 저장하기 위해 사용하는 SO(Scriptable Object)

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
        itemSaveDataList = itemInitData.itemSaveDataList;
        itemSaveDataDict = itemInitData.itemSaveDataList.ToDictionary(x => x.itemID);
    }

    /// <summary>
    /// Json데이터에 아이템 정보 저장
    /// isForce는 강제적으로 Save 시킬 때 사용, true면 반드시 저장
    /// </summary>
    /// <param name="jsonData"></param>
    public void Save(JsonData jsonData, bool isForce = false)
    {
        // 데이터 변경이 되지 않았고, isForce가 false면 리턴
        if (!isDirty && !isForce)
            return;

        jsonData.itemList = itemSaveDataList; // json데이터 변경

        isDirty = false; // 데이터 변경 여부 초기화
    }

    /// <summary>
    /// Json데이터를 받아와 데이터 갱신을 한다.
    /// </summary>
    /// <param name="jsonData"></param>
    public void Load(JsonData jsonData)
    {
        itemSaveDataList = jsonData.itemList;
        itemSaveDataDict = itemSaveDataList.ToDictionary(x => x.itemID);

        isLoadData = true;
    }

    /// <summary>
    /// 아이템 저장 데이터를 원본이 아닌 복사본을 받는다.
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public async UniTask<ItemSaveData> GetReadOnly(int itemID)
    {
        var data = await Get(itemID);

        if (data == null)
            return null;

        // 새로 복사본을 만들어 그 데이터를 리턴한다. 
        return new ItemSaveData
        {
            itemID = itemID,
            itemCount = data.itemCount,
        };
    }

    /// <summary>
    /// 아이템 구매 및 사용
    /// </summary>
    public async UniTask UpdatePurchase(int itemID, int value)
    {
        var itemSaveData = await Get(itemID); // 아이템 ID에 해당하는 아이템SaveData를 가져온다.

        // 값이 없으면 추가 후 진행
        if(itemSaveData == null)
        {
            itemSaveData = new ItemSaveData
            {
                itemID = itemID,
                itemCount = 0
            };
            itemSaveDataList.Add(itemSaveData);
            itemSaveDataDict.Add(itemID, itemSaveData);
        }

        itemSaveData.itemCount += value; // 아이템 개수 증가

        isDirty = true; // 데이터 변경되었으니 true로 변경
    }

    /// <summary>
    /// 아이템 ID에 해당하는 아이템SaveData를 가져온다.
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    private async UniTask<ItemSaveData> Get(int itemID)
    {
        // 데이터 Load를 하지 않았으면 대기
        while (!isLoadData)
        {
            await UniTask.Yield();
        }

        if (itemSaveDataDict.TryGetValue(itemID, out var itemSaveData))
        {
            return itemSaveData;
        }
        return null;
    }
}
