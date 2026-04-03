using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockItemData", menuName = "KGC/BlockData/BlockItemData")]
public class BlockItemData : ScriptableObject
{
    public List<EffectConfig> effectConfigList;

    public void Use()
    {
        foreach (EffectConfig config in effectConfigList)
        {
            config.effectData.Execute(config);
        }
    }
}
