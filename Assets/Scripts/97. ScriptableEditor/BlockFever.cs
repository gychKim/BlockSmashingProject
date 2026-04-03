using UnityEngine;

[CreateAssetMenu(fileName = "BlockFever", menuName = "KGC/BlockResult/BlockFever")]
public class BlockFever : BlockResult
{
    public float duration;

    public override void Use(MainGameController controller)
    {
        EventManager.Instance.Publish(EffectType.Fever, duration);
    }
}
