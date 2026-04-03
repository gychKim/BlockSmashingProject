using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 퀘스트 진행도
/// </summary>
[Serializable]
public class QuestProgress
{
    public int questID; // 퀘스트 ID
    public int progress; // 진행도
    public bool completed; // 완료 여부
    public bool claimed; // 승인 여부

    public string conditionStateJson; // 퀘스트 조건 저장 데이터
}

/// <summary>
/// 퀘스트 저장 데이터
/// </summary>
[Serializable]
public class QuestSaveData // QuestDailyState
{
    public long nextResetUtcTicks; // utc ticks
    public List<int> activeQuestIDList; // 활성화할 퀘스트 아이디 리스트
    public List<QuestProgress> progressList; // 퀘스트 진행도 리스트
    public List<int> recentQuestIDList; // 최근 퀘스트 아이디 리스트
    public string lastAssignedKstDate; // 마지막 Kst Date e.g., "YYYY-MM-DD"
}
