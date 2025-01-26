using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Singleton will be destroyed between scenes
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    public static T Instance { get { return _instance; } }

    protected void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }
}
