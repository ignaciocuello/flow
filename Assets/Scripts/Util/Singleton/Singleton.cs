using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component {

    public static T Instance;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Initialize();

            Instance = GetComponent<T>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Initialize()
    {
    }
}
