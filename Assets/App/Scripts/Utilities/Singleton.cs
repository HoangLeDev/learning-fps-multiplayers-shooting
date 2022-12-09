using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// Singleton for mono behavior object
/// Inherit this class to make the child become singleton
/// </summary>
/// <typeparam name="T">Type of child object</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T I
    {
        get
        {
            if (instance == null)
                instance = (T)FindObjectOfType(typeof(T));

            return instance;
        }
    }


    protected virtual void Awake()
    {
        instance = this as T;
        if (instance = null)
            Destroy(gameObject);
    }
}

public class SingletonNetworking<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
{
    private static T instance;

    public static T I
    {
        get
        {
            if (instance == null)
                instance = (T)FindObjectOfType(typeof(T));

            return instance;
        }
    }


    protected virtual void Awake()
    {
        instance = this as T;
        if (instance = null)
            Destroy(gameObject);
    }
}