using UnityEngine;

[CreateAssetMenu(fileName = "EffectItemSpawnChance", menuName = "KGC/EffectData/EffectItemSpawnChance")]
public class EffectItemSpawnChance : EffectData
{
    public override void Execute(EffectConfig config)
    {
        EventManager.Instance.Publish(EffectType.ItemSpawnChance, config.value, config.duration);
    }
}
