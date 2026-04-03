using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "DirectionData", menuName = "KGC/DirectionDatas/StageDirectionData")]
public class DirectionData : ScriptableObject
{
    public int stage;
    public PlayableDirector director;
}
