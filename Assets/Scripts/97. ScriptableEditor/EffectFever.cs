using UnityEngine;

[CreateAssetMenu(fileName = "EffectFever", menuName = "KGC/EffectData/EffectFever")]
public class EffectFever : EffectData
{
    public override void Execute(EffectConfig config)
    {
        EventManager.Instance.Publish(EffectType.Fever, config.duration);
    }
}
