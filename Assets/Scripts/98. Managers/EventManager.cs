using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{

    private Dictionary<Type, Dictionary<Enum, List<IEvent>>> eventDict = new(); // Type을 키로 하는 이유는, Enum값은 사실 int로 처리되므로 중복값 방지를 위해 Type을 키로 한다.

    /// <summary>
    /// Type에 맞는 Dict을 생성 및 리턴받는다.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="enumType"></param>
    /// <returns></returns>
    private Dictionary<Enum, List<IEvent>> GetOrCreateEventDict<TEnum>() where TEnum : Enum
    {
        var enumType = typeof(TEnum); // Enum의 타입을 가져온다.

        // eventDict이 enumType을 지니고 있지 않다면, 추가한다. 
        if(!eventDict.TryGetValue(enumType, out var dict))
        {
            dict = new Dictionary<Enum, List<IEvent>>();
            eventDict[enumType] = dict;
        }

        // type에 알맞은 dict 리턴
        return dict;
    }

    /// <summary>
    /// 게임 이벤트 구독
    /// 이벤트 전용 ID를 리턴한다.
    /// </summary>
    /// <param name="MainGameEventType"></param>
    /// <param name="action"></param>
    /// <param name="unityEvent"></param>
    public Guid Subscribe<TEnum>(TEnum eventType, Action action = null, UnityEvent unityEvent = null) where TEnum : Enum
    {
        var eventData = new EventClass(action, unityEvent);

        var dict = GetOrCreateEventDict<TEnum>();

        // 매개변수로 받은 eventType(enum)은 값 형식이라 바로 참조형식인 Enum으로 바로 형변환할 수 없다(컴파일 오류 발생)
        // 따라서 먼저 object로 박싱을 한 후, Enum으로 언박싱 해줘야 한다.
        var key = (Enum)(object)eventType;

        // key에 해당하는 값이 없다면, 새로운 리스트를 생성 후, Dict에 Add한다.
        // 존재하면, 그 값을 out으로 가져와 list에 담는다
        if (!dict.TryGetValue(key, out var list))
        {
            list = new List<IEvent>();
            dict.Add(key, list);
        }

        // list에 eventData를 넣는다.
        list.Add(eventData);

        // 이벤트 ID 리턴
        return eventData.EventID;
    }

    public Guid Subscribe<TEnum, T>(TEnum eventType, Action<T> action = null, UnityEvent<T> unityEvent = null) where TEnum : Enum
    {
        var eventData = new EventClass<T>(action, unityEvent);

        var dict = GetOrCreateEventDict<TEnum>();

        // 매개변수로 받은 eventType(enum)은 값 형식이라 바로 참조형식인 Enum으로 바로 형변환할 수 없다(컴파일 오류 발생)
        // 따라서 먼저 object로 박싱을 한 후, Enum으로 언박싱 해줘야 한다.
        var key = (Enum)(object)eventType;

        // key에 해당하는 값이 없다면, 새로운 리스트를 생성 후, Dict에 Add한다.
        // 존재하면, 그 값을 out으로 가져와 list에 담는다
        if (!dict.TryGetValue(key, out var list))
        {
            list = new List<IEvent>();
            dict.Add(key, list);
        }

        // list에 eventData를 넣는다.
        list.Add(eventData);

        // 이벤트 ID 리턴
        return eventData.EventID;
    }

    public Guid Subscribe<TEnum, T1, T2>(TEnum eventType, Action<T1, T2> action = null, UnityEvent<T1, T2> unityEvent = null) where TEnum : Enum
    {
        var eventData = new EventClass<T1, T2>(action, unityEvent);

        var dict = GetOrCreateEventDict<TEnum>();

        // 매개변수로 받은 eventType(enum)은 값 형식이라 바로 참조형식인 Enum으로 바로 형변환할 수 없다(컴파일 오류 발생)
        // 따라서 먼저 object로 박싱을 한 후, Enum으로 언박싱 해줘야 한다.
        var key = (Enum)(object)eventType;

        // key에 해당하는 값이 없다면, 새로운 리스트를 생성 후, Dict에 Add한다.
        // 존재하면, 그 값을 out으로 가져와 list에 담는다
        if (!dict.TryGetValue(key, out var list))
        {
            list = new List<IEvent>();
            dict.Add(key, list);
        }

        // list에 eventData를 넣는다.
        list.Add(eventData);

        // 이벤트 ID 리턴
        return eventData.EventID;
    }

    /// <summary>
    /// 게임 이벤트 구독 해제
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="eventID"></param>
    public void Unsubscribe<TEnum>(TEnum eventType, Guid eventID) where TEnum : Enum
    {
        if (eventID == Guid.Empty)
            return;

        var dict = GetOrCreateEventDict<TEnum>();
        var key = (Enum)(object)eventType;

        if (!dict.TryGetValue(key, out var list))
            return;

        list.RemoveAll(evt => evt.EventID == eventID); // 조건에 해당하는 이벤트를 이벤트 리스트에서 제거한다.
    }

    /// <summary>
    /// 이벤트 호출
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="eventType"></param>
    public void Publish<TEnum>(TEnum eventType) where TEnum : Enum
    {
        var dict = GetOrCreateEventDict<TEnum>();
        var key = (Enum)(object)eventType;

        if (!dict.TryGetValue(key, out var list))
            return;

        foreach (var evt in list)
            evt.Publish();
    }

    public void Publish<TEnum, T>(TEnum eventType, T t) where TEnum : Enum
    {
        var dict = GetOrCreateEventDict<TEnum>();
        var key = (Enum)(object)eventType;

        if (!dict.TryGetValue(key, out var list))
            return;

        foreach (var evt in list)
            evt.Publish(t);
    }
    public void Publish<TEnum, T1, T2>(TEnum eventType, T1 t1, T2 t2) where TEnum : Enum
    {
        var dict = GetOrCreateEventDict<TEnum>();
        var key = (Enum)(object)eventType;

        if (!dict.TryGetValue(key, out var list))
            return;

        foreach (var evt in list)
            evt.Publish(t1, t2);
    }

    //#region 게임 이벤트
    ///// <summary>
    ///// 게임 이벤트 구독
    ///// 이벤트 전용 ID를 리턴한다.
    ///// </summary>
    ///// <param name="MainGameEventType"></param>
    ///// <param name="action"></param>
    ///// <param name="unityEvent"></param>
    //public Guid Subscribe<TEnum>(TEnum MainGameEventType, Action action = null, UnityEvent unityEvent = null) where TEnum : Enum
    //{
    //    var eventData = new EventClass(action, unityEvent);

    //    // MainGameEventType에 해당하는 값이 없다면, 새로운 리스트를 생성 후, Dict에 Add한다.
    //    // 존재하면, 그 값을 out으로 가져와 list에 담는다

    //    if (!eventDict.TryGetValue(MainGameEventType, out var list))
    //    {
    //        list = new List<IEvent>();
    //        eventDict.Add(MainGameEventType, list);
    //    }

    //    list.Add(eventData); // list에 eventData를 넣는다.

    //    return eventData.EventID;
    //}

    //public Guid Subscribe<T>(MainGameEventType MainGameEventType, Action<T> action = null, UnityEvent<T> unityEvent = null)
    //{
    //    var eventData = new EventClass<T>(action, unityEvent);

    //    // MainGameEventType에 해당하는 값이 없다면, 새로운 리스트를 생성 후, Dict에 Add한다.
    //    // 존재하면, 그 값을 out으로 가져와 list에 담는다
    //    if (!gameEventDict.TryGetValue(MainGameEventType, out var list))
    //    {
    //        list = new List<IEvent>();
    //        gameEventDict.Add(MainGameEventType, list);
    //    }

    //    list.Add(eventData); // list에 eventData를 넣는다.

    //    return eventData.EventID;
    //}

    //public Guid Subscribe<T1, T2>(MainGameEventType MainGameEventType, Action<T1, T2> action = null, UnityEvent<T1, T2> unityEvent = null)
    //{
    //    var eventData = new EventClass<T1, T2>(action, unityEvent);

    //    // MainGameEventType에 해당하는 값이 없다면, 새로운 리스트를 생성 후, Dict에 Add한다.
    //    // 존재하면, 그 값을 out으로 가져와 list에 담는다
    //    if (!gameEventDict.TryGetValue(MainGameEventType, out var list))
    //    {
    //        list = new List<IEvent>();
    //        gameEventDict.Add(MainGameEventType, list);
    //    }

    //    list.Add(eventData); // list에 eventData를 넣는다.

    //    return eventData.EventID;
    //}

    ///// <summary>
    ///// 게임 이벤트 구독 해제
    ///// </summary>
    ///// <param name="MainGameEventType"></param>
    ///// <param name="action"></param>
    ///// <param name="unityEvent"></param>
    //public void Unsubscribe(MainGameEventType MainGameEventType, Guid eventID)
    //{
    //    if (eventID == Guid.Empty)
    //        return;

    //    if (!gameEventDict.TryGetValue(MainGameEventType, out var list))
    //        return;

    //    list.RemoveAll(evt => evt.EventID == eventID); // 조건에 해당하는 이벤트를 이벤트 리스트에서 제거한다.
    //}

    ///// <summary>
    ///// 게임 이벤트 호출
    ///// </summary>
    ///// <param name="MainGameEventType"></param>
    //public void Publish(MainGameEventType MainGameEventType)
    //{
    //    if (!gameEventDict.TryGetValue(MainGameEventType, out var list))
    //        return;

    //    foreach (var evt in list)
    //        evt.Publish();
    //}
    //public void Publish<T>(MainGameEventType MainGameEventType, T t)
    //{
    //    if (!gameEventDict.TryGetValue(MainGameEventType, out var list))
    //        return;

    //    foreach (var evt in list)
    //        evt.Publish(t);
    //}
    //public void Publish<T1, T2>(MainGameEventType MainGameEventType, T1 t1, T2 t2)
    //{
    //    if (!gameEventDict.TryGetValue(MainGameEventType, out var list))
    //        return;

    //    foreach (var evt in list)
    //        evt.Publish(t1, t2);
    //}
    //#endregion

    //#region UI이벤트
    ///// <summary>
    ///// UI 이벤트 구독
    ///// </summary>
    ///// <param name="uiEventType"></param>
    ///// <param name="action"></param>
    ///// <param name="unityEvent"></param>
    //public Guid Subscribe(UIEventType uiEventType, Action action = null, UnityEvent unityEvent = null)
    //{
    //    var eventData = new EventClass(action, unityEvent);

    //    // MainGameEventType에 해당하는 값이 없다면, 새로운 리스트를 생성 후, Dict에 Add한다.
    //    // 존재하면, 그 값을 out으로 가져와 list에 담는다
    //    if (!uiEventDict.TryGetValue(uiEventType, out var list))
    //    {
    //        list = new List<IEvent>();
    //        uiEventDict.Add(uiEventType, list);
    //    }

    //    list.Add(eventData); // list에 eventData를 넣는다.

    //    return eventData.EventID;
    //}

    //public Guid Subscribe<T>(UIEventType uiEventType, Action<T> action = null, UnityEvent<T> unityEvent = null)
    //{
    //    var eventData = new EventClass<T>(action, unityEvent);

    //    // MainGameEventType에 해당하는 값이 없다면, 새로운 리스트를 생성 후, Dict에 Add한다.
    //    // 존재하면, 그 값을 out으로 가져와 list에 담는다
    //    if (!uiEventDict.TryGetValue(uiEventType, out var list))
    //    {
    //        list = new List<IEvent>();
    //        uiEventDict.Add(uiEventType, list);
    //    }

    //    list.Add(eventData); // list에 eventData를 넣는다.

    //    return eventData.EventID;
    //}

    //public Guid Subscribe<T1, T2>(UIEventType uiEventType, Action<T1, T2> action = null, UnityEvent<T1, T2> unityEvent = null)
    //{
    //    var eventData = new EventClass<T1, T2>(action, unityEvent);

    //    // MainGameEventType에 해당하는 값이 없다면, 새로운 리스트를 생성 후, Dict에 Add한다.
    //    // 존재하면, 그 값을 out으로 가져와 list에 담는다
    //    if (!uiEventDict.TryGetValue(uiEventType, out var list))
    //    {
    //        list = new List<IEvent>();
    //        uiEventDict.Add(uiEventType, list);
    //    }

    //    list.Add(eventData); // list에 eventData를 넣는다.

    //    return eventData.EventID;
    //}

    ///// <summary>
    ///// UI 이벤트 구독 해제
    ///// </summary>
    ///// <param name="uiEventType"></param>
    ///// <param name="action"></param>
    ///// <param name="unityEvent"></param>
    //public void Unsubscribe(UIEventType uiEventType, Guid eventID)
    //{
    //    if (eventID == Guid.Empty)
    //        return;

    //    if (!uiEventDict.TryGetValue(uiEventType, out var list))
    //        return;

    //    list.RemoveAll(evt => evt.EventID == eventID); // 조건에 해당하는 이벤트를 이벤트 리스트에서 제거한다.
    //}

    ///// <summary>
    ///// UI 이벤트 호출
    ///// </summary>
    ///// <param name="uiEventType"></param>
    //public void Publish(UIEventType uiEventType)
    //{
    //    if (!uiEventDict.TryGetValue(uiEventType, out var list))
    //        return;

    //    foreach (var evt in list)
    //        evt.Publish();
    //}
    //public void Publish<T>(UIEventType uiEventType, T t)
    //{
    //    if (!uiEventDict.TryGetValue(uiEventType, out var list))
    //        return;

    //    foreach (var evt in list)
    //        evt.Publish(t);
    //}
    //public void Publish<T1, T2>(UIEventType uiEventType, T1 t1, T2 t2)
    //{
    //    if (!uiEventDict.TryGetValue(uiEventType, out var list))
    //        return;

    //    foreach (var evt in list)
    //        evt.Publish(t1, t2);
    //}
    //#endregion

    private void OnDestroy()
    {
        eventDict.Clear();
        //gameEventDict.Clear();
        //uiEventDict.Clear();
    }
}
