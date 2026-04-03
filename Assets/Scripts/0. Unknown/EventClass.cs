using System;
using UnityEngine;
using UnityEngine.Events;

public class EventClass : IEvent
{
    public Guid EventID => eventID;
    private Guid eventID;

    public Action Action => action;
    readonly private Action action;

    public UnityEvent UnityEvent => unityEvent;

    readonly private UnityEvent unityEvent;

    /// <summary>
    /// 이벤트 등록
    /// </summary>
    /// <param name="action"></param>
    /// <param name="unityEvent"></param>
    public EventClass(Action action, UnityEvent unityEvent)
    {
        eventID = Guid.NewGuid(); // 이벤트 고유한 ID 생성

        this.action = action;
        this.unityEvent = unityEvent;
    }

    /// <summary>
    /// 이벤트 호출
    /// </summary>
    public void Publish(params object[] args)
    {
        action?.Invoke();
        unityEvent?.Invoke();
    }
}

public class EventClass<T> : IEvent
{
    public Guid EventID => eventID;
    private Guid eventID;

    public Action<T> Action => action;
    readonly private Action<T> action;

    public UnityEvent<T> UnityEvent => unityEvent;
    readonly private UnityEvent<T> unityEvent;

    /// <summary>
    /// 이벤트 등록
    /// </summary>
    /// <param name="action"></param>
    /// <param name="unityEvent"></param>
    public EventClass(Action<T> action, UnityEvent<T> unityEvent)
    {
        eventID = Guid.NewGuid();
        this.action = action;
        this.unityEvent = unityEvent;
    }

    /// <summary>
    /// 이벤트 호출
    /// </summary>
    public void Publish(params object[] args)
    {
        if(args.Length > 0 && args[0] is T type)
        {
            action?.Invoke(type);
            unityEvent?.Invoke(type);
        }
    }
}

public class EventClass<T1, T2> : IEvent
{
    public Guid EventID => eventID;
    private Guid eventID;

    public Action<T1, T2> Action => action;
    readonly private Action<T1, T2> action;

    public UnityEvent<T1, T2> UnityEvent => unityEvent;
    readonly private UnityEvent<T1, T2> unityEvent;

    /// <summary>
    /// 이벤트 등록
    /// </summary>
    /// <param name="action"></param>
    /// <param name="unityEvent"></param>
    public EventClass(Action<T1, T2> action, UnityEvent<T1, T2> unityEvent)
    {
        eventID = Guid.NewGuid();
        this.action = action;
        this.unityEvent = unityEvent;
    }

    /// <summary>
    /// 이벤트 호출
    /// </summary>
    public void Publish(params object[] args)
    {
        if (args.Length > 1 && args[0] is T1 t1 && args[1] is T2 t2)
        {
            action?.Invoke(t1, t2);
            unityEvent?.Invoke(t1, t2);
        }
    }
}
