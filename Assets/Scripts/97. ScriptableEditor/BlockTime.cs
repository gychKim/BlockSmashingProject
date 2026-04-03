using UnityEngine;

[CreateAssetMenu(fileName = "BlockTime", menuName = "KGC/BlockResult/BlockTime")]
public class BlockTime : BlockResult
{
    public int value;

    public override void Use(MainGameController controller)
    {
        EventManager.Instance.Publish(EffectType.Time, value);
    }
}
