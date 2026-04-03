using System;
using UnityEngine;

public class Entitiy : MonoBehaviour
{
    [SerializeField]
    private BaseStats baseStats;

    public Stats Stats { get; private set; }

    public Item CurrentItem = new Item();
    private void Awake()
    {
        Stats = new Stats(new StatsMediator(), baseStats);
    }

    private void Update()
    {
        Stats.Mediator.Update(Time.deltaTime);

        DebugX.Log(Stats.ToString());
    }

    public void UseItem() // 1
    {
        CurrentItem.UseItem(this);
    }
}

[Serializable]
public class Item
{
    public enum OperatorType { Add, Remove, Multiply, }

    [SerializeField] StatType statType = StatType.Attack;
    [SerializeField] OperatorType operatorType = OperatorType.Add;
    [SerializeField] int value = 10;
    [SerializeField] float duration = 5f;

    public void UseItem(Entitiy entitiy) // 2
    {
        StatModifier modifier = operatorType switch
        {
            OperatorType.Add => new BasicStatModifier(statType, duration, v => v + value), // 9
            OperatorType.Multiply => new BasicStatModifier(statType, duration, v => v * value), // 9
            OperatorType.Remove => new BasicStatModifier(statType, duration, v => v - value), // 9
            _ => throw new System.NotImplementedException(),
        };

        entitiy.Stats.Mediator.AddModifier(modifier);
    }

}
