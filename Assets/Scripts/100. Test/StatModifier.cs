using System;
using System.Numerics;
public abstract class StatModifier : IDisposable
{
    public bool MarkedForRemoval { get; private set; }

    public event Action<StatModifier> OnDispose = delegate { }; // delegate { }는 뭐지? > 이벤트 호출 시 구독한 개체가 없더라도 null 참조 예외 없이 안전하게 호출할 수 있다.

    readonly CountdownTimer timer;

    protected StatModifier(float duration) // 3
    {
        if (duration <= 0)
            return;

        timer = new CountdownTimer(duration);
        timer.OnTimerStop += () => MarkedForRemoval = true;
        timer.Start();
    }
    
    public void Update(float deltaTime) => timer?.Tick(deltaTime);
    public abstract void Handle(object sender, Query query);
    public void Dispose()
    {
        OnDispose?.Invoke(this);
    }
}

public class BasicStatModifier : StatModifier
{
    readonly StatType statType;
    readonly Func<int, int> operation;

    public BasicStatModifier(StatType statType, float duration, Func<int, int> operation) : base(duration) // 4
    {
        this.statType = statType;
        this.operation = operation;
    }

    public override void Handle(object sender, Query query) // 8 > 매개변수 Query는 baseStat의 데이터가 들어간다.
    {
        if(statType == query.statType)
        {
            query.value = operation(query.value);
        }
    }
}

#region 사용안함 INumber<T>이 있어야 할 만 할듯
public abstract class Operator<T> where T : struct
{
    public abstract T Operation(T value1, T value2);
}
public class AddOperator<T> : Operator<T> where T : struct
{
    public override T Operation(T value1, T value2)
    {
        if ((value1 is int leftValue) && (value2 is int rightValue))
        {
            return (T)(object)(leftValue + rightValue);
        }
        return default(T);
    }
}
public class RemoveOperator<T> : Operator<T> where T : struct
{
    public override T Operation(T value1, T value2)
    {
        if ((value1 is int leftValue) && (value2 is int rightValue))
        {
            return (T)(object)(leftValue - rightValue);
        }
        return default(T);
    }
}
public class MultiplyOperator<T> : Operator<T> where T : struct
{
    public override T Operation(T value1, T value2)
    {
        if ((value1 is int leftValue) && (value2 is int rightValue))
        {
            return (T)(object)(leftValue * rightValue);
        }
        return default(T);
    }
}
#endregion
