using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class QuestModel : BaseModel
{
    public ReactiveCollection<QuestItem> Items { get; } = new();
    public override void Start()
    {
        Items.Clear();

        // 일일퀘스트 리스트
        var questList = QuestManager.Instance.ActiveProgresses;

        // 퀘스트 생성 및 초기화
        foreach (var progress in questList)
        {
            var item = PoolManager.Instance.Rent(PoolObjectType.QuestItem).GetComponent<QuestItem>();

            item.Init(progress); // 아이템 초기화

            Items.Add(item);
        }
    }

    public override void Destroy()
    {
        
    }
}
