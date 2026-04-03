using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestPool", menuName = "KGC/Quest/QuestPool")]
public class QuestPoolSO : ScriptableObject
{
    [Min(1)]
    public int dailyQuestCount = 4; // 일일 퀘스트 개수

    [Min(0)]
    public int cooldownDays = 2; // 퀘스트 갱신 일수

    public List<QuestDataSO> questList = new List<QuestDataSO>(); // 퀘스트 리스트
}
