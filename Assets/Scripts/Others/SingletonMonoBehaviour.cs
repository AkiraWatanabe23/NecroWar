using System;
using UnityEngine;

/// <summary> SingletonMonoBehaviourの基底クラス </summary>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = default;

    protected new abstract bool DontDestroyOnLoad { get; }

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Type type = typeof(T);
                _instance = (T)FindObjectOfType(type);
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        if (DontDestroyOnLoad) { DontDestroyOnLoad(gameObject); }
    }
}