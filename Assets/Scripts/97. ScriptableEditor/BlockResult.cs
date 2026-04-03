using UnityEngine;

/// <summary>
/// 아이템 블록
/// </summary>
public abstract class BlockResult : ScriptableObject
{
    // public List<Effect> effectList;
    public EffectType resultType;
    public abstract void Use(MainGameController controller = null);
}
