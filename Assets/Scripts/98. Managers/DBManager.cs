using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;

public class DBManager : Singleton<DBManager>, IDataAwake
{
    #region 인게임
    private Dictionary<BlockType, BlockGameData> blockGameDataDict = new();
    [SerializeField]
    private List<BlockGameData> blockGameDataDB;

    private Dictionary<EffectType, BlockResult> blockResultDict = new();
    [SerializeField]
    private List<BlockResult> blockResultDB;

    private Dictionary<int, StageData> stageDataDict = new();
    [SerializeField]
    private List<StageData> stageDataDB;
    #endregion

    private Dictionary<int, CharacterData> characterDataDict = new();
    [SerializeField]
    private List<CharacterData> characterDataDB;

    private Dictionary<int, BlockData> blockDataDict = new();
    [SerializeField]
    private List<BlockData> blockDataDB;

    /// <summary>
    /// 아이템 데이터
    /// </summary>
    private Dictionary<int, BlockItemData> itemDataDict = new();
    [SerializeField]
    private List<BlockItemData> itemDataDB;


    private Dictionary<int, ShopItemDataSO> shopItemDataDict = new();
    [SerializeField]
    private List<ShopItemDataSO> shopItemDataDB;

    private Dictionary<ShopType, ShopSO> shopDataDict = new();
    [SerializeField]
    private List<ShopSO> shopDataDB;

    public UniTask DataAwakeAsync(CancellationToken cancelToken)
    {
        for (int i = 0; i < Enum.GetValues(typeof(BlockType)).Length; i++)
        {
            blockGameDataDict.Add((BlockType)i, Resources.Load<BlockGameData>("Scriptable/BlockGameData/" + (BlockType)i));
        }

        //// 블록 게임 데이터 Dict 초기화
        //foreach(var blockGameData in blockGameDataDB)
        //{
        //    blockGameDataDict.Add(blockGameData.blockType, blockGameData);
        //}

        // 블록 효과 데이터 Dict 초기화
        //foreach(var blockResultData in blockResultDB)
        //{
        //    blockResultDict.Add(blockResultData.resultType, blockResultData);
        //}
        for (int i = 0; i < Enum.GetValues(typeof(EffectType)).Length; i++)
        {
            blockResultDict.Add((EffectType)i, Resources.Load<BlockResult>("Scriptable/BlockResult/" + (EffectType)i));
        }

        // 스테이지 데이터 Dict 초기화
        foreach (var stageData in stageDataDB)
        {
            stageDataDict.Add(stageData.currentStage, stageData);
        }

        // 캐릭터 데이터 Dict 초기화
        foreach (var characterData in characterDataDB)
        {
            characterDataDict.Add(characterData.characterID, characterData);
        }

        // 블록 데이터 Dict 초기화
        foreach (var blockData in blockDataDB)
        {
            blockDataDict.Add(blockData.blockID, blockData);
        }

        for (int i = 2001; i < (int)ShopItemType.End; i++) // 2001은 아이템 ID의 시작점
        {
            itemDataDict.Add(i, Resources.Load<BlockItemData>("Scriptable/ItemData/ShopItemData/" + (ShopItemType)i));
        }

        foreach (var shopItemData in shopItemDataDB)
        {
            shopItemDataDict.Add(shopItemData.ID, shopItemData);
        }

        foreach (var shopData in shopDataDB)
        {
            shopDataDict.Add(shopData.ShopType, shopData);
        }

        return UniTask.CompletedTask;
    }

    /// <summary>
    /// 블록 게임 데이터(SO)를 가져온다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public BlockGameData GetBlockGameData(BlockType type)
    {
        if(blockGameDataDict.TryGetValue(type, out var blockGameData))
        {
            return blockGameData;
        }

        return null;
    }
    public BlockResult GetBlockResultData(EffectType type)
    {
        if (blockResultDict.TryGetValue(type, out var blockResultData))
        {
            return blockResultData;
        }

        return null;
    }
    public StageData GetStageData(int stage)
    {
        if (stageDataDict.TryGetValue(stage, out var stageData))
        {
            return stageData;
        }

        return null;
    }

    public CharacterData GetCharacterData(int characterID)
    {
        if (characterDataDict.TryGetValue(characterID, out var characterData))
        {
            return characterData;
        }

        return null;
    }

    public BlockData GetBlockData(int blockID)
    {
        if (blockDataDict.TryGetValue(blockID, out var blockData))
        {
            return blockData;
        }

        return null;
    }

    public BlockItemData GetItemData(int itemID)
    {
        if (itemDataDict.TryGetValue(itemID, out var itemData))
        {
            return itemData;
        }

        return null;
    }

    public ShopItemDataSO GetShopItemData(int itemID)
    {
        if (shopItemDataDict.TryGetValue(itemID, out var shopItemData))
        {
            return shopItemData;
        }

        return null;
    }

    public ShopSO GetShopData(ShopType type)
    {
        if (shopDataDict.TryGetValue(type, out var shopData))
        {
            return shopData;
        }

        return null;
    }

    //public DirectionData GetDirectionData(int stage)
    //{
    //    if (directionDataDict.TryGetValue(stage, out var directionData))
    //    {
    //        return directionData;
    //    }

    //    return null;
    //}
}
