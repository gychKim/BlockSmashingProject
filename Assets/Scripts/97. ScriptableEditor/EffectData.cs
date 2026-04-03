using UnityEngine;

public abstract class EffectData : ScriptableObject
{
    public int ID; // 효과 아이디
    public abstract void Execute(EffectConfig config); // 효과 실행
}
