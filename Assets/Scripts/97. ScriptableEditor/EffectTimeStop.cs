using UnityEngine;

[CreateAssetMenu(fileName = "EffectTimeStop", menuName = "KGC/EffectData/EffectTimeStop")]
public class EffectTimeStop : EffectData
{
    public override void Execute(EffectConfig config)
    {
        EventManager.Instance.Publish(EffectType.TimeStop, config.duration);
    }
}
