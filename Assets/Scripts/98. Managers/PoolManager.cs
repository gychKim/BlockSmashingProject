using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx.Toolkit;
using UnityEngine;

[Serializable]
public class PoolClass
{
    public PoolObjectType poolObjectType;
    public Component poolObject;
}
public class PoolManager : Singleton<PoolManager>, IDataAwake
{
    [SerializeField]
    private List<PoolClass> prefabList;

    private Dictionary<PoolObjectType, ObjectPooling<Component>> poolDict = new Dictionary<PoolObjectType, ObjectPooling<Component>>();

    public UniTask DataAwakeAsync(CancellationToken cancelToken)
    {
        foreach (var prefab in prefabList)
        {
            poolDict.Add(prefab.poolObjectType, new ObjectPooling<Component>(prefab.poolObject));
        }

        return UniTask.CompletedTask;
    }


    public Component Rent(PoolObjectType poolObjType)
    {
        if (poolDict.TryGetValue(poolObjType, out var pool))
        {
            return pool.Rent();
        }
        return null;
    }

    public void Return(PoolObjectType poolObjType, Component obj)
    {
        if(poolDict.TryGetValue(poolObjType, out var pool))
        {
            pool.Return(obj);
        }
    }

    public void Clear(PoolObjectType poolObjType)
    {
        if (poolDict.TryGetValue(poolObjType, out var pool))
        {
            pool.Clear();
        }
    }
}

public class ObjectPooling<T> : ObjectPool<T> where T : Component
{
    private T prefab;
    private Transform parent;
    public ObjectPooling(T prefab, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
    }
    protected override T CreateInstance()
    {
        var obj = GameObject.Instantiate(prefab);
        obj.transform.SetParent(parent, true);
        //obj.transform.parent = parent;
        return obj;
    }
}
