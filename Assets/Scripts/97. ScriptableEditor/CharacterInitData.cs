using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInitData", menuName = "KGC/Initdata/CharacterInitData")]
public class CharacterInitData : ScriptableObject
{
    // 최초 게임 실행 시 캐릭터 상태를 저장하고 있는 리스트
    public List<CharacterSaveData> characterSaveDataList = new List<CharacterSaveData>();
}
