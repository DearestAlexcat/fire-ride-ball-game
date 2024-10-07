using System.Collections.Generic;
using UnityEngine;

public class PoolerService<T> where T : Component
{
    Pooler<T> pooler;
    Dictionary<int, T> cacheObjects;

    public PoolerService(T gate, int poolSize)
    {
        pooler = new Pooler<T>(gate, poolSize);
        cacheObjects = new Dictionary<int, T>();
    }

    public T Get(int cacheKey, bool resetOrientation = true)
    {
        var item = pooler.Get(resetOrientation);
        cacheObjects.Add(cacheKey, item);
        return item;
    }

    public T Get(int cacheKey, Vector3 position, Quaternion rotation)
    {
        var item = pooler.Get(position, rotation);
        cacheObjects.Add(cacheKey, item);
        return item;
    }

    public void Clear()
    {
        cacheObjects.Clear();
        pooler.Clear();
    }

    public void Free(int cacheKey)
    {
        if(cacheObjects.ContainsKey(cacheKey))
        {
            pooler.Free(cacheObjects[cacheKey]);
            cacheObjects.Remove(cacheKey);
        }
    }
}