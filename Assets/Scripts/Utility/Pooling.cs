using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Pooling : PersistentSingleton<Pooling>
//{
//    //public Dictionary<string, Pool> pools;

//    void AddPool(string _name, GameObject prefab) 
//    {
//        pools.Add(_name, new Pool(prefab));
//    }
//}

public struct Pool 
{
    public int Count => objects.Count;

    Transform parent;
    GameObject prefab;
    List<GameObject> objects;

    public List<int> inUse;
    public Dictionary<int, float> disposeTimers;

    public Pool(Transform _parent, GameObject _prefab, int startSize = 10) 
    {
        parent = _parent;
        prefab = _prefab;
        objects = new List<GameObject>();
        inUse = new List<int>();
        disposeTimers = new Dictionary<int, float>();

        Expand(startSize);
    }

    //public bool Update(float _timePassed) 
    //{
    //    for (int i = 0; i < inUse.Count; i++)
    //    {
    //        if (disposeTimers.ContainsKey(inUse[i]))
    //        {
    //            disposeTimers[inUse[i]] -= _timePassed;

    //            if (disposeTimers[inUse[i]] <= 0)
    //            {
    //                //Debug.Log("Disposing " + inUse[i]);
    //                Dispose(inUse[i]);
    //                i--;
    //            }
    //        }
    //    }

    //    return disposeTimers.Count > 0;
    //}

    public void Expand(int count) 
    {
        for (int i = 0; i < count; i++)
        {
            objects.Add(Object.Instantiate(prefab, parent));
            objects[Count - 1].SetActive(false);
        }
    }

    public GameObject GetObject(int i)
    {
        return objects[i];
    }

    private void LockObject(int i) 
    {
        inUse.Add(i);
    }

    private void UnlockObject(int i)
    {
        disposeTimers.Remove(i);
        inUse.Remove(i);
    }

    /// <returns>The next available index of a pool object</returns>
    private int NextAvailable() 
    {
        for (int i = 0; i < Count; i++)
        {
            if (!inUse.Contains(i))
            {
                return i;
            }
        }

        //don't use magic numbers here...
        Expand(1);
        return Count - 1;
    }

    public GameObject Fetch() 
    {
        int i = NextAvailable();
        LockObject(i);
        objects[i].SetActive(true);
        return objects[i];
    }

    /// <summary>
    /// For this function to work, you need to called the pooling Update function at some point
    /// </summary>
    //public void DelayedDispose(GameObject poolObject, float _delay) 
    //{
    //    int i = objects.FindIndex(x => x == poolObject);

    //    if (disposeTimers.ContainsKey(i)) return;

    //    disposeTimers.Add(i, _delay);
    //}

    public void Dispose(GameObject poolObject) 
    {
        int i = objects.FindIndex(x => x == poolObject);

        if (i == -1)
        {
            Object.Destroy(poolObject);
            return;
        }
        Dispose(i);
    }

    public void Dispose(int idx)
    {
        if (idx >= objects.Count) return;
        UnlockObject(idx);
        objects[idx].SetActive(false);
    }

    internal void DisposeAll()
    {
        while (inUse.Count > 0)
        {
            Dispose(inUse[0]);
        }
    }

    //internal void DisposeAll(float _delay)
    //{
    //    for (int i = 0; i < inUse.Count; i++)
    //    {
    //        DelayedDispose(objects[inUse[i]], _delay);
    //    }
    //}
}
