using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>, IDataAwake
{
#if UNITY_EDITOR
    public bool isEncryption = false;
#endif
    public CharacterSaveManager CharacterSaveManager => characterSaveManager;
    [SerializeField]
    private CharacterSaveManager characterSaveManager;

    public BlockSaveManager BlockSaveManager => blockSaveManager;
    [SerializeField]
    private BlockSaveManager blockSaveManager;

    public GameStateSaveManager GameStateSaveManager => gameStateSaveManager;
    [SerializeField]
    private GameStateSaveManager gameStateSaveManager;

    public ItemSaveManager ItemSaveManager => itemSaveManager;
    [SerializeField]
    private ItemSaveManager itemSaveManager;

    public QuestSaveManager QuestSaveManager => questSaveManager;
    [SerializeField]
    private QuestSaveManager questSaveManager;

    private JsonData jsonData; // JsonData 저장 변수

    /// <summary>
    /// 데이터 변화(Update)가 존재하는지
    /// </summary>
    public bool IsDirty => characterSaveManager.IsDirty || blockSaveManager.IsDirty || gameStateSaveManager.IsDirty || itemSaveManager.IsDirty || questSaveManager.IsDirty; // 더티플래그 > 캐릭터, 게임 상태 등의 데이터 변화가 있다면 true, 없으면 false

    public async UniTask DataAwakeAsync(CancellationToken cancelToken)
    {
        await Load();  // DataManager가 그 누구보다 빨리 JsonData를 Load해야한다. Awake에서 데이터를 필요로 하는 경우가 있으니, Awake보다 빠르게 가져와야 한다.
    }

    /// <summary>
    /// Json데이터를 Save한다.
    /// isForce는 강제적으로 Save 시킬 때 사용, true면 반드시 저장
    /// </summary>
    public async UniTask Save(bool isForce = false)
    {
        characterSaveManager.Save(jsonData, isForce);
        blockSaveManager.Save(jsonData, isForce);
        gameStateSaveManager.Save(jsonData, isForce);
        itemSaveManager.Save(jsonData, isForce);
        questSaveManager.Save(jsonData, isForce);

        // 비동기로 저장한다.
        await JsonManager.SaveAsync(jsonData, "jsonData.sav", false);
        DebugX.GoldLog("Json데이터 Save");
    }

    /// <summary>
    /// Json데이터를 Load한다
    /// </summary>
    public async UniTask Load()
    {
        // 비동기 Load를 await하고 끝나면(Load된 데이터를) data에 넣는다.
        var data = await JsonManager.LoadAsync<JsonData>("jsonData.sav", false);

        if (data == null)
        {
            await InitJsonData();
            return;
        }

        jsonData = data;

        characterSaveManager.Load(jsonData);
        blockSaveManager.Load(jsonData);
        gameStateSaveManager.Load(jsonData);
        itemSaveManager.Load(jsonData);
        questSaveManager.Load(jsonData);

        DebugX.CyanLog("Json데이터 Load");
    }

    /// <summary>
    /// Json데이터 초기화
    /// </summary>
    private async UniTask InitJsonData()
    {
        // InitData SO 기반으로 전부 초기화
        jsonData = new JsonData();

        // 세이브매니저들 초기화
        characterSaveManager.Init();
        blockSaveManager.Init();
        gameStateSaveManager.Init();
        itemSaveManager.Init();
        questSaveManager.Init();

        // JsonData 초기화
        jsonData.characterList = characterSaveManager.CharacterSaveDataList.Select(x => new CharacterSaveData
        {
            characterID = x.characterID,
            currentSkinIndex = x.currentSkinIndex,
            isPurchase = x.isPurchase
        }).ToList();

        jsonData.blockList = blockSaveManager.BlockSaveDataList.Select(x => new BlockSaveData
        {
            blockID = x.blockID,
            blockName = x.blockName,
            isPurchase = x.isPurchase
        }).ToList();

        jsonData.itemList = itemSaveManager.ItemSaveDataList.Select(x => new ItemSaveData
        {
            itemID = x.itemID,
            itemCount = x.itemCount,
        }).ToList();

        jsonData.questSaveData = questSaveManager.QuestSaveData;
        //jsonData.gameState = await gameStateSaveManager.GetReadOnly();

        // JsonData 저장
        await Save(true);

        // 재 로드
        await Load();
    }

    private void OnApplicationPause(bool pause)
    {
        // 일시정지(앱 백그라운드로 이동)이며, 변경되었으면 저장
        if (pause && IsDirty)
        {
            Save(true).Forget();
        }
    }

    private void OnDestroy()
    {
        // 변경되었으면 저장
        if(IsDirty)
        {
            Save(true).Forget();
        }

        // 나중에

        // 일정 주기 자동 저장
        // 스테이지 클리어 시점 저장

        // 등등도 구현해보자.
    }
}
