using UnityEngine;

[CreateAssetMenu(fileName = "EffectScore", menuName = "KGC/EffectData/EffectScore")]
public class EffectScore : EffectData
{
    public override void Execute(EffectConfig config)
    {
        EventManager.Instance.Publish(EffectType.Score, (int)config.value);
    }
}
