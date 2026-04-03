using System;

/// <summary>
/// 아이템 중재자
/// </summary>
public abstract class ItemModifier
{
    public EffectType ItemType { get; protected set; } // 아이템 종류

    public bool MarkedForRemoval { get; protected set; } // 제거 조건이 만족하였는지

    public event Action<ItemModifier> OnDispose = delegate { }; // delegate { }는 뭐지? > 이벤트 호출 시 구독한 개체가 없더라도 null 참조 예외 없이 안전하게 호출할 수 있다.

    /// <summary>
    /// 중재자 갱신
    /// </summary>
    /// <param name="deltaTime"></param>
    public abstract void Update(float value);

    /// <summary>
    /// 중재자 관리/실행
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="query"></param>
    public abstract void Handle(object sender, ItemQuery query);

    /// <summary>
    /// 중재자 제거
    /// </summary>
    public void Dispose()
    {
        DebugX.Log("Modifier 제거");
        OnDispose?.Invoke(this);
    }
}

/// <summary>
/// 제거 조건이 시간인 아이템 중재자
/// </summary>
public abstract class TimeModifier : ItemModifier
{
    readonly CountdownTimer timer;

    public TimeModifier(float duration)
    {
        if (duration <= 0)
            return;

        timer = new CountdownTimer(duration);
        timer.OnTimerStop += () => MarkedForRemoval = true;
        timer.Start();
    }

    public override void Update(float deltaTime) => timer?.Tick(deltaTime);

}

/// <summary>
/// 제거 조건이 시간(Task)인 아이템 중재자
/// </summary>
public abstract class TimeTaskModifier : ItemModifier
{
    readonly CountdownTimerTask timer;

    public TimeTaskModifier(float duration)
    {
        if (duration <= 0)
            return;

        timer = new CountdownTimerTask(duration);
        timer.OnTimerStop += () => MarkedForRemoval = true;
        timer.Start();
    }
}

/// <summary>
/// 제거 조건이 횟수인 아이템 중재자
/// </summary>
public abstract class CountModifier : ItemModifier
{
    private int remainingCount;

    public CountModifier(int count)
    {
        remainingCount = count;
    }

    public override void Update(float value)
    {
        remainingCount -= (int)value;

        if (remainingCount <= 0)
            MarkedForRemoval = true;
    }
}

public class TimeItemModifier : TimeModifier
{
    readonly Func<float, float> operation;

    public TimeItemModifier(EffectType itemType, float duration, Func<float, float> operation) : base(duration)
    {
        ItemType = itemType;
        this.operation = operation;
    }

    public override void Handle(object sender, ItemQuery query)
    {
        if (ItemType == query.itemType)
        {
            query.value = operation(query.value);
        }
    }
}

public class TimeTaskItemModifier : TimeTaskModifier
{
    readonly Func<float, float> operation;

    public TimeTaskItemModifier(EffectType itemType, float duration, Func<float, float> operation) : base(duration)
    {
        ItemType = itemType;
        this.operation = operation;
    }

    public override void Handle(object sender, ItemQuery query)
    {
        if (ItemType == query.itemType)
        {
            query.value = operation(query.value);
        }
    }

    /// <summary>
    /// 아무것도 안한다. 시간 흐름은 Task에서 진행함
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="NotImplementedException"></exception>
    public override void Update(float value)
    {
        throw new NotImplementedException();
    }
}

public class CountItemModifier : CountModifier
{
    readonly Func<float, float> operation;

    public CountItemModifier(EffectType itemType, int count, Func<float, float> operation) : base(count)
    {
        ItemType = itemType;
        this.operation = operation;
    }

    public override void Handle(object sender, ItemQuery query)
    {
        if (ItemType == query.itemType)
        {
            query.value = operation(query.value);
        }
    }
}

public class ItemQuery
{
    public readonly EffectType itemType;
    public float value;

    public ItemQuery(EffectType itemType, int value)
    {
        this.itemType = itemType;
        this.value = value;
    }
}

