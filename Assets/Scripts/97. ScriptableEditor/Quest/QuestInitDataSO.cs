using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInitData", menuName = "KGC/Initdata/QuestInitData")]
public class QuestInitDataSO : ScriptableObject
{
    // 최초 게임 실행 시 아이템 상태를 저장하고 있는 리스트
    public QuestSaveData questSaveData= new QuestSaveData();
}
