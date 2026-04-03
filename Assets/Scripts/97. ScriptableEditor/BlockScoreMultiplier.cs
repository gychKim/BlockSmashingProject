using UnityEngine;


[CreateAssetMenu(fileName = "BlockScoreMultiplier", menuName = "KGC/BlockResult/BlockScoreMultiplier")]
public class BlockScoreMultiplier : BlockResult
{
    public float duration; // 지속시간
    public int value; // 배율

    public override void Use(MainGameController controller)
    {
        EventManager.Instance.Publish(EffectType.ScoreMultiplier, value, duration);
    }
}
