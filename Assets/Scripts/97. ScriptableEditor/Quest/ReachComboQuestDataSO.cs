using UnityEngine;

[CreateAssetMenu(fileName = "ReachComboCondition", menuName = "KGC/Quest/Condition/ReachCombo")]
public class ReachComboQuestConditionSO : QuestConditionDataSO
{
    public string questItemName;

    private class Runtime : IQuestConditionRuntime
    {
        private ReachComboQuestConditionSO conditionSO;
        private int currentCombo;
        private readonly string questItemName;

        public Runtime(ReachComboQuestConditionSO conditionSO)
        {
            this.conditionSO = conditionSO;
            this.questItemName = conditionSO.questItemName;
        }

        public void OnStart()
        {

        }

        public string GetItemName()
        {
            return questItemName;
        }

        public void OnEvent(in QuestEvent questEvent)
        {
            if (questEvent.type == QuestConditionType.ComboReach)
            {
                currentCombo = questEvent.newCombo;
            }
        }

        public string SaveState()
        {
            return currentCombo.ToString();
        }

        public void LoadState(string json)
        {
            if (int.TryParse(json, out var currentCombo))
            {
                this.currentCombo = currentCombo;
            }
        }

        public int GetProgress()
        {
            return currentCombo;
        }

        public bool IsCompleted(int targetValue)
        {
            return currentCombo >= targetValue;
        }
    }
    public override IQuestConditionRuntime CreateRuntime()
    {
        return new Runtime(this);
    }
}
