using UnityEngine;

public enum StatType { Attack, Defense, }
public class Stats
{
    readonly StatsMediator mediator;
    readonly BaseStats baseStats;

    public StatsMediator Mediator => mediator;

    public int Attack // 6 현재 공격력
    {
        get
        {
            var q = new Query(StatType.Attack, baseStats.attack);
            mediator.PerformQuery(this, q); // 이 때 query는 캐릭터의 기본 스탯의 데이터를 mediator에 전달하고
            return q.value; // 10, 이 후 빠져나오면(Func<int, int>를 거치고 난 후) value가 baseStat + itemValue 의 값이된다.
        }
    }
    public int Defense // 6
    {
        get
        {
            var q = new Query(StatType.Defense, baseStats.defense);
            mediator.PerformQuery(this, q);
            return q.value; // 10
        }
    }

    public Stats(StatsMediator mediator, BaseStats baseStats)
    {
        this.mediator = mediator;
        this.baseStats = baseStats;
    }

    public override string ToString() => $"Attack : {Attack}, Defense : {Defense}";
}
