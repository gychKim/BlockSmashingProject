using UnityEngine;

[CreateAssetMenu(fileName = "BlockTimeStop", menuName = "KGC/BlockResult/BlockTimeStop")]
public class BlockTimeStop : BlockResult
{
    public float duration;
    public override void Use(MainGameController controller)
    {
        EventManager.Instance.Publish(EffectType.TimeStop, duration);
    }
}
