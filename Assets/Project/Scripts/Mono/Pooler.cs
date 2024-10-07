﻿using UnityEngine;
using System.Collections.Generic;

public class Pooler<T> where T : Component
{
	protected Queue<T> m_FreeInstances = new Queue<T>();
	protected T m_Original;
	protected Transform m_parent;

	public Pooler(T original, int initialSize, Transform parent = null)
	{
		m_Original = original;
		m_FreeInstances = new Queue<T>(initialSize);
		m_parent = parent;

		for (int i = 0; i < initialSize; ++i)
		{
			T obj = Object.Instantiate(m_Original, m_parent);
			obj.gameObject.SetActive(false);
			//obj.name = Time.realtimeSinceStartup.ToString();

            m_FreeInstances.Enqueue(obj);
		}
	}

    private T Get()
    {
        T ret = m_FreeInstances.Count > 0 ? m_FreeInstances.Dequeue() : Object.Instantiate(m_Original, m_parent);
        ret.gameObject.SetActive(true);
        return ret;
    }

    public T Get(bool resetOrientation = true)
	{
        return resetOrientation ? Get(Vector3.zero, Quaternion.identity) : Get();
	}

    public T Get(Vector3 position, Quaternion rotation)
	{
		T ret = Get();
		ret.gameObject.transform.position = position;
		ret.gameObject.transform.rotation = rotation;

		return ret;
	}

	public void Clear()
    {
		int count = m_FreeInstances.Count;

		for (int i = 0; i < count; i++)
        {
			Object.Destroy(m_FreeInstances.Dequeue().gameObject);
		}
    }

	public void Free(T obj)
	{
		//obj.gameObject.transform.SetParent(null);
		obj.gameObject.SetActive(false);
		m_FreeInstances.Enqueue(obj);
	}
}
