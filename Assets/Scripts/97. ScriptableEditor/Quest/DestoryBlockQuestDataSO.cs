using UnityEngine;

[CreateAssetMenu(fileName = "DestoryBlockCondition", menuName = "KGC/Quest/Condition/DestoryBlock")]
public class DestoryBlockQuestConditionSO : QuestConditionDataSO
{
    public BlockType blockType;
    public string questItemName;

    private class Runtime : IQuestConditionRuntime
    {
        private readonly DestoryBlockQuestConditionSO conditionSO;
        private int count;
        private readonly string questItemName;

        public Runtime(DestoryBlockQuestConditionSO conditionSO)
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
            if (questEvent.type == QuestConditionType.DestoryBlock && questEvent.blockType == conditionSO.blockType)
                count++;
        }

        public int GetProgress()
        {
            return count;
        }

        public bool IsCompleted(int targetValue)
        {
            return count >= targetValue;
        }

        public string SaveState()
        {
            return count.ToString();
        }

        public void LoadState(string json)
        {
            if(int.TryParse(json, out var count))
            {
                this.count = count;
            }
        }
    }
    public override IQuestConditionRuntime CreateRuntime()
    {
        return new Runtime(this);
    }
}
