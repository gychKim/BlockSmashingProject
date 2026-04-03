using System;
using UnityEngine.Pool;

[Serializable]
public static class ClassPoolSystem<T> where T : class, IPoolable, new()
{
    public static readonly ObjectPool<T> dict = new ObjectPool<T>(

        createFunc: () => new T(),
        actionOnGet: poolable => { poolable.Get(); },
        actionOnRelease: poolable => { poolable.Release(); },
        actionOnDestroy: poolable => { poolable.Destroy(); },
        collectionCheck: true,
        defaultCapacity: 20,
        maxSize: 100

        );
}
