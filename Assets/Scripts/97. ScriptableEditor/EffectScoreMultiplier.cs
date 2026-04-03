using UnityEngine;

[CreateAssetMenu(fileName = "EffectScoreMultiplier", menuName = "KGC/EffectData/EffectScoreMultiplier")]
public class EffectScoreMultiplier : EffectData
{
    public override void Execute(EffectConfig config)
    {
        EventManager.Instance.Publish(EffectType.ScoreMultiplier, (int)config.value, config.duration);
    }
}
