using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDeSpawn : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("DeSpawn", 1f);
    }

    private void DeSpawn()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}