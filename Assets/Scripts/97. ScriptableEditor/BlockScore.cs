using UnityEngine;

/// <summary>
/// 점수 블록 > 기본 블록
/// </summary>
[CreateAssetMenu(fileName = "BlockScore", menuName = "KGC/BlockResult/BlockScore")]
public class BlockScore : BlockResult
{
    public bool value;

    public override void Use(MainGameController controller)
    {
        EventManager.Instance.Publish(EffectType.Score, value);
    }
}
