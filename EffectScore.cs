using UnityEngine;

[CreateAssetMenu(fileName = "EffectScore", menuName = "KGC/EffectData/EffectScore")]
public class EffectScore : EffectData
{
    public override void Execute(MainGameModel model, EffectConfig config)
    {
        model.SetScore(config.value == 1);
    }
}
