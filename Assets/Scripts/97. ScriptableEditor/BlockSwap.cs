using UnityEngine;

[CreateAssetMenu(fileName = "BlockSwap", menuName = "KGC/BlockResult/BlockSwap")]
public class BlockSwap : BlockResult
{
    public override void Use(MainGameController controller)
    {
        controller.BlockSwap();
    }
}
