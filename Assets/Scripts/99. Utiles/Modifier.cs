using System;
using System.Collections.Generic;
using UniRx;

public abstract class Modifier<T> : IDisposable
{
    public bool MarkedForRemoval { get; private set; }

    public event Action<Modifier<T>> OnDispose = delegate { };

    readonly CountdownTimer timer;

    protected Modifier(float duration)
    {
        if (duration <= 0)
            return;

        timer = new CountdownTimer(duration);
        timer.OnTimerStop += () => MarkedForRemoval = true;
        timer.Start();
    }

    public void Update(float deltaTime) => timer?.Tick(deltaTime);
    public abstract void Handle(object sender, Query<T> query);
    public void Dispose()
    {
        OnDispose?.Invoke(this);
    }
}

public class FloatModifier<T> : Modifier<T>
{
    readonly T type;
    readonly Func<FloatReactiveProperty, FloatReactiveProperty> operation;

    /// <summary>
    /// 사용하고자 하는 계산식은 생성하는 측에서 결정한다.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="duration"></param>
    /// <param name="operation"></param>
    public FloatModifier(T type, float duration, Func<FloatReactiveProperty, FloatReactiveProperty> operation) : base(duration)
    {
        this.type = type;
        this.operation = operation;
    }

    public override void Handle(object sender, Query<T> query)
    {
        if (EqualityComparer<T>.Default.Equals(type, query.Type) && query is FloatQuery<T> floatQuery)
        {
            floatQuery.value = operation(floatQuery.value);
        }
    }
}

public class IntModifier<T> : Modifier<T>
{
    readonly T type;
    readonly Func<int, int> operation;

    /// <summary>
    /// 사용하고자 하는 계산식은 생성하는 측에서 결정한다.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="duration"></param>
    /// <param name="operation"></param>
    public IntModifier(T type, int duration, Func<int, int> operation) : base(duration)
    {
        this.type = type;
        this.operation = operation;
    }

    public override void Handle(object sender, Query<T> query)
    {
        if (EqualityComparer<T>.Default.Equals(type, query.Type) && query is IntQuery<T> intQuery)
        {
            intQuery.value = operation(intQuery.value);
        }
    }
}
