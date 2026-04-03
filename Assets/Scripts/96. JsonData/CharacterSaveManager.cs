using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 런타임 시, 캐릭터 데이터 수정 및 저장을 담당
/// </summary>
[System.Serializable]
public class CharacterSaveManager : ISaveManager
{
    // 현재 지니고 있는 캐릭터 리스트
    // 유니티 Inspector에서 편집할 때 이용
    // 만약, Odin같은 에셋을 사용한다면 굳이 리스트 데이터는 필요가 없다.
    // 하지만 NewtonJson을 사용하지 않는다면, Dictionary를 Json화 시킬 수 없으므로 리스트를 사용하는게 맞다.
    public List<CharacterSaveData> CharacterSaveDataList => characterSaveDataList;
    private List<CharacterSaveData> characterSaveDataList = new();

    private Dictionary<int, CharacterSaveData> characterSaveDataDict = new();

    [SerializeField]
    private CharacterInitData characterInitData; // 최초 게임 실행 시, characterJsonData에 최초데이터를 저장하기 위해 사용하는 SO(Scriptable Object)

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
        characterSaveDataList = characterInitData.characterSaveDataList;
        characterSaveDataDict = characterInitData.characterSaveDataList.ToDictionary(x => x.characterID);
    }

    /// <summary>
    /// Json데이터에 캐릭터 정보 저장
    /// isForce는 강제적으로 Save 시킬 때 사용, true면 반드시 저장
    /// </summary>
    /// <param name="jsonData"></param>
    public void Save(JsonData jsonData, bool isForce = false)
    {
        // 데이터 변경이 되지 않았고, isForce가 false면 리턴
        if (!isDirty && !isForce)
            return;

        jsonData.characterList = characterSaveDataList; // json데이터 변경

        isDirty = false; // 데이터 변경 여부 초기화
    }

    /// <summary>
    /// Json데이터를 받아와 데이터 갱신을 한다.
    /// </summary>
    /// <param name="jsonData"></param>
    public void Load(JsonData jsonData)
    {
        characterSaveDataList = jsonData.characterList;
        characterSaveDataDict = characterSaveDataList.ToDictionary(x => x.characterID);

        isLoadData = true;
    }

    /// <summary>
    /// 캐릭터 저장 데이터를 원본이 아닌 복사본을 받는다.
    /// </summary>
    /// <param name="characterID"></param>
    /// <returns></returns>
    public async UniTask<CharacterSaveData> GetReadOnly(int characterID)
    {
        var data = await Get(characterID);

        if (data == null)
            return null;

        // 새로 복사본을 만들어 그 데이터를 리턴한다. 
        return new CharacterSaveData
        {
            characterID = characterID,
            characterName = data.characterName,
            currentSkinIndex = data.currentSkinIndex,
            purchaseSkinArr = data.purchaseSkinArr,
            isPurchase = data.isPurchase,
        };
    }

    /// <summary>
    /// 캐릭터 현재 스킨 Index 변경
    /// </summary>
    public async UniTask UpdateSkinIndex(int characterID, int skinIndex)
    {
        var characterSaveData = await Get(characterID); // 캐릭터 ID에 해당하는 캐릭터SaveData를 가져온다.

        // 가져온 데이터와 동일하면 바로 리턴
        if (characterSaveData.currentSkinIndex == skinIndex)
            return;

        characterSaveData.currentSkinIndex = skinIndex; // 가져온 캐릭터의 SkinIndex를 변경한다.
        isDirty = true; // 데이터 변경되었으니 true로 변경
    }

    /// <summary>
    /// 캐릭터 구매 여부 변경
    /// </summary>
    public async UniTask UpdatePurchase(int characterID, bool value)
    {
        var characterSaveData = await Get(characterID); // 캐릭터 ID에 해당하는 캐릭터SaveData를 가져온다.

        if (characterSaveData.isPurchase == value)
            return;

        characterSaveData.isPurchase = value; // 가져온 캐릭터의 구매 여부를 변경한다.

        isDirty = true; // 데이터 변경되었으니 true로 변경
    }

    /// <summary>
    /// 스킨 구매 여부 변경
    /// </summary>
    public async UniTask UpdateSkinPurchase(int characterID, int skinIndex, bool value)
    {
        var characterSaveData = await Get(characterID); // 캐릭터 ID에 해당하는 캐릭터SaveData를 가져온다.

        if (characterSaveData.currentSkinIndex == skinIndex) // 현재 스킨일 시 바로 리턴
            return;

        characterSaveData.purchaseSkinArr[skinIndex] = value; // skinIndex에 해당하는 구매여부 값을 value로 설정한다.

        isDirty = true; // 데이터 변경되었으니 true로 변경
    }

    /// <summary>
    /// 캐릭터 ID에 해당하는 캐릭터SaveData를 가져온다.
    /// </summary>
    /// <param name="characterID"></param>
    /// <returns></returns>
    private async UniTask<CharacterSaveData> Get(int characterID)
    {
        // 데이터 Load를 하지 않았으면 대기
        while (!isLoadData)
        {
            await UniTask.Yield();
        }

        if (characterSaveDataDict.TryGetValue(characterID, out var characterSaveData))
        {
            return characterSaveData;
        }
        return null;
    }
}
