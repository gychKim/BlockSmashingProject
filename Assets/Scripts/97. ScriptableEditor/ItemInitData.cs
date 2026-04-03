using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemInitData", menuName = "KGC/Initdata/ItemInitData")]
public class ItemInitData : ScriptableObject
{
    // 최초 게임 실행 시 아이템 상태를 저장하고 있는 리스트
    public List<ItemSaveData> itemSaveDataList = new List<ItemSaveData>();
}
