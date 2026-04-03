using UnityEngine;

[CreateAssetMenu(fileName = "EffectShield", menuName = "KGC/EffectData/EffectShield")]
public class EffectShield : EffectData
{
    public override void Execute(EffectConfig config)
    {
        EventManager.Instance.Publish(EffectType.Shield, (int)config.value);
    }
}
