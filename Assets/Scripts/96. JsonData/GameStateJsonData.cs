using System;

/// <summary>
/// 게임상태데이터 > 게임 시작 시, 저장된 게임 상태를 담는 데이터
/// </summary>
[Serializable]
public class GameStateSaveData
{
    // 게임 진행 시간,

    // 현재 캐릭터 ID
    public int currentCharacterID;

    // 현재 블록 ID
    public int currentBlockID;

    // 현재 골드
    public int currentGold;

    // 현재 다이아
    public int currentDia;

    // 최고 스테이지

    // 최고 점수

}
