using UnityEngine;

[CreateAssetMenu(fileName = "BlockShield", menuName = "KGC/BlockResult/BlockShield")]
public class BlockShield : BlockResult
{
    public int value;

    public override void Use(MainGameController controller)
    {
        EventManager.Instance.Publish(EffectType.Shield, value);
    }
}
