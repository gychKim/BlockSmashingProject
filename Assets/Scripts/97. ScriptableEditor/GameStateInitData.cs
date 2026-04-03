using UnityEngine;

[CreateAssetMenu(fileName = "GameStateInitData", menuName = "KGC/Initdata/GameStateInitData")]
public class GameStateInitData : ScriptableObject
{
    // GameStateJsonData의 데이터 내용(개수)과 1:1 관계를 이뤄야 한다.

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
