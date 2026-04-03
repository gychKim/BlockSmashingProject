using System;

/// <summary>
/// 블록 세이브 데이터 > 블록마다 저장해야 하는 데이터
/// </summary>
[Serializable]
public class BlockSaveData
{
    public int blockID; // 블록 ID

    public string blockName; // 블록 이름

    public bool isPurchase; // 구매 여부
}
