using System;

/// <summary>
/// 캐릭터 세이브 데이터 > 캐릭터마다 저장해야 하는 데이터
/// </summary>
[Serializable]
public class CharacterSaveData
{
    public int characterID; // 캐릭터 ID

    public string characterName; // 캐릭터 이름

    public int currentSkinIndex; // 캐릭터 현재 스킨 Index

    public bool[] purchaseSkinArr = new bool[4]; // Index에 해당하는 스킨 구매 여부

    public bool isPurchase; // 구매 여부

}
