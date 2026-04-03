using Cysharp.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class GameStateSaveManager
{
    //public int CurrentCharacterID => currentCharacterID;
    //private int currentCharacterID;  // 현재 캐릭터 ID

    //public int CurrentBlockID => currentBlockID;
    //private int currentBlockID; // 현재 블록 ID

    [SerializeField]
    private GameStateInitData gameStateInitData; // 최초 게임 실행 시, gameStateJsonData에 최초데이터를 저장하기 위해 사용하는 SO(Scriptable Object)

    private GameStateSaveData gameStateSaveData;
    public bool IsDirty => isDirty;
    private bool isDirty = false; // 변경되었는지 확인

    public bool IsLoadData => isLoadData;
    private bool isLoadData; // 데이터를 Load했는지 여부
    /// <summary>
    /// 초기화
    /// </summary>
    public void Init()
    {
        gameStateSaveData = new GameStateSaveData
        {
            currentCharacterID = gameStateInitData.currentCharacterID,
            currentBlockID = gameStateInitData.currentBlockID,
            currentGold = gameStateInitData.currentGold,
            currentDia = gameStateInitData.currentDia,
        };
        isLoadData = true;
    }

    /// <summary>
    /// Json데이터에 게임상태 정보 저장
    /// </summary>
    /// <param name="jsonData"></param>
    public void Save(JsonData jsonData, bool isForce = false)
    {
        // 데이터 변경이 되지 않았으면 리턴
        if (!isDirty && !isForce)
            return;

        jsonData.gameState = gameStateSaveData;

        isDirty = false; // 데이터 변경 여부 초기화
    }

    /// <summary>
    /// Json데이터를 받아와 데이터 갱신을 한다.
    /// </summary>
    /// <param name="jsonData"></param>
    public void Load(JsonData jsonData)
    {
        gameStateSaveData = jsonData.gameState;
        isLoadData = true;
    }

    /// <summary>
    /// 게임 상태 데이터의 복사본을 받는다.
    /// </summary>
    /// <returns></returns>
    public async UniTask<GameStateSaveData> GetReadOnly()
    {
        var data = await Get();

        if (data == null)
            return null;

        return new GameStateSaveData
        {
            currentCharacterID = data.currentCharacterID,
            currentBlockID = data.currentBlockID,
            currentGold = data.currentGold,
        };
    }

    /// <summary>
    /// 캐릭터 ID 갱신
    /// </summary>
    /// <param name="characterID"></param>
    public async UniTask UpdateCurrentCharacterID(int characterID)
    {
        var data = await Get();

        if (data == null)
            return;

        data.currentCharacterID = characterID;

        isDirty = true; // 데이터 변경되었으니 true
    }

    /// <summary>
    /// 블록 ID 갱신
    /// </summary>
    /// <param name="blockID"></param>
    public async UniTask UpdateBlockID(int blockID)
    {
        var data = await Get();

        if (data == null)
            return;

        data.currentBlockID = blockID;

        isDirty = true; // 데이터 변경되었으니 true
    }

    public async UniTask UpdateGold(int currentGold)
    {
        var data = await Get();

        if (data == null)
            return;

        data.currentGold = currentGold;

        isDirty = true; // 데이터 변경되었으니 true
    }
    public async UniTask UpdateDia(int currentDia)
    {
        var data = await Get();

        if (data == null)
            return;

        data.currentDia = currentDia;

        isDirty = true; // 데이터 변경되었으니 true
    }

    /// <summary>
    /// 게임상태데이터 Get > gameStateSaveData 필드를 그대로 사용하기보다 뭔가 가독성이 좋음
    /// </summary>
    /// <returns></returns>
    private async UniTask<GameStateSaveData> Get()
    {
        // 데이터 Load를 하지 않았으면 대기
        while (!isLoadData)
        {
            await UniTask.Yield();
        }

        // 데이터 Load를 했으면 데이터 리턴
        return gameStateSaveData;
    }
}
