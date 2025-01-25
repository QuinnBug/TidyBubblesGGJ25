using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Singleton will persist through scene changes
/// </summary>
public abstract class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
{
    private static T _instance;
    public static T Instance { get { return _instance; } }

    protected void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
            DontDestroyOnLoad(_instance);
        }
        else if (_instance != this) 
        {
            Destroy(this);
        }
    }
}
