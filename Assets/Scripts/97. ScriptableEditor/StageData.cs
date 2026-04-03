using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "KGC/Stage/StageData")]
public class StageData : ScriptableObject
{
    public int currentStage; // 현재 스테이지

    public string stageName; // 스테이지 이름

    public string stageDescription; // 스테이지 설명

    public Sprite stageImage; // 스테이지 이미지

    public int time; // 현재 스테이지의 제한 시간
    public int goalScore; // 현재 스테이지의 목표 점수

    public int blockScore; // 현재 스테이지의 기본 블록 하나 당 점수

    public StageRewardData reward; // 스테이지 보상
}
