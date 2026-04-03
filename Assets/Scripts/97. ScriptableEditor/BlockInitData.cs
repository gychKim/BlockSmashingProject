using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockInitData", menuName = "KGC/Initdata/BlockInitData")]
public class BlockInitData : ScriptableObject
{
    // 최초 게임 실행 시 블록 상태를 저장하고 있는 리스트
    public List<BlockSaveData> blockSaveDataList = new List<BlockSaveData>();
}
