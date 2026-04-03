using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 브로커 체인 패턴 적용
/// 브로커 체인 같은 경우, 모든 수정자를 매 프레임이나 업데이트마다 변경해야할 수 있기에, 모든 수정자에 대한 참조 지점이 있는것이 좋다.
/// </summary>
public class StatsMediator
{
    readonly LinkedList<StatModifier> modifiers = new LinkedList<StatModifier>();

    public event EventHandler<Query> Queries; 
    public void PerformQuery(object sender, Query query) => Queries?.Invoke(sender, query); // 7

    public void AddModifier(StatModifier modifier) // 5
    {
        modifiers.AddLast(modifier);
        Queries += modifier.Handle;
        modifier.OnDispose += (_) =>
        {
            modifiers.Remove(modifier);
            Queries -= modifier.Handle;
        };
    }

    public void Update(float deltaTime)
    {
        var node = modifiers.First;
        while(node != null)
        {
            var modifier = node.Value;
            modifier.Update(deltaTime);
            node = node.Next;
        }

        node = modifiers.First;
        while (node != null)
        {
            var nextNode = node.Next;

            if(node.Value.MarkedForRemoval)
            {
                modifiers.Remove(node);
                node.Value.Dispose();
            }

            node = nextNode;
        }
    }
}

public class Query
{
    public readonly StatType statType;
    public int value;

    public Query(StatType statType, int value)
    {
        this.statType = statType;
        this.value = value;
    }
}
