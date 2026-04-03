using UnityEngine;

[CreateAssetMenu(fileName = "EffectTime", menuName = "KGC/EffectData/EffectTime")]
public class EffectTime : EffectData
{
    public override void Execute(EffectConfig config)
    {
        EventManager.Instance.Publish(EffectType.Time, (int)config.value);
    }
}
