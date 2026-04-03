using System.Collections.Generic;
using System;

public class ItemMediator
{
    readonly LinkedList<ItemModifier> modifiers = new LinkedList<ItemModifier>();

    public event EventHandler<ItemQuery> Queries;
    public void PerformQuery(object sender, ItemQuery query) => Queries?.Invoke(sender, query); // 7

    /// <summary>
    /// Modifier 추가
    /// </summary>
    /// <param name="modifier"></param>
    public void AddModifier(ItemModifier modifier)
    {
        modifiers.AddLast(modifier);
        Queries += modifier.Handle;
        modifier.OnDispose += (_) =>
        {
            modifiers.Remove(modifier);
            Queries -= modifier.Handle;
        };
    }

    /// <summary>
    /// type에 해당하는 모든 Modifier를 제거한다.
    /// </summary>
    /// <param name="type"></param>
    public void RemoveAllModifier(EffectType type)
    {
        var node = modifiers.First;
        while (node != null)
        {
            var nextNode = node.Next;

            if(node.Value.ItemType == type)
            {
                modifiers.Remove(node);
                node.Value.Dispose();
            }

            node = nextNode;
        }
    }

    /// <summary>
    /// 갱신
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Update(float deltaTime)
    {
        var node = modifiers.First;
        while (node != null)
        {
            var modifier = node.Value;
            modifier.Update(deltaTime);
            node = node.Next;
        }

        node = modifiers.First;
        while (node != null)
        {
            var nextNode = node.Next;

            if (node.Value.MarkedForRemoval)
            {
                modifiers.Remove(node);
                node.Value.Dispose();
            }

            node = nextNode;
        }
    }

    /// <summary>
    /// Mediator 초기화
    /// </summary>
    public void Clear()
    {
        var node = modifiers.First;
        while (node != null)
        {
            modifiers.Remove(node);
            node.Value.Dispose();
        }

        modifiers.Clear();
        Queries = null;
    }
}
