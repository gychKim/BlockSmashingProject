using UnityEngine;

[CreateAssetMenu(fileName = "QuestConditionDataSO", menuName = "KGC/Quest/QuestConditionDataSO")]
public abstract class QuestConditionDataSO : ScriptableObject
{
    public abstract IQuestConditionRuntime CreateRuntime();
}
