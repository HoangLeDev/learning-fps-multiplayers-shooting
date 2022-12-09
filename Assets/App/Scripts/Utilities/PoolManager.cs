using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject poolObject;
    private bool isInit;
    private Transform poolManager;
    private List<GameObject> spawnPoolList;


    private void InitPoolSpawn(GameObject prefab)
    {
        poolManager = GameObject.Find("PoolManager").transform;
        isInit = true;
        poolObject = prefab;
        spawnPoolList = new List<GameObject>();
        GameObject obj = (GameObject)Instantiate(poolObject);
        obj.transform.SetParent(poolManager);
        obj.SetActive(false);
        spawnPoolList.Add(obj);
    }

    private void AddMorePoolObjects()
    {
        GameObject obj = (GameObject)Instantiate(poolObject);
        obj.transform.SetParent(poolManager);
        spawnPoolList.Add(obj);
    }

    public void PoolSpawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!isInit)
            InitPoolSpawn(prefab);

        for (int i = 0; i < spawnPoolList.Count; i++)
        {
            if (!spawnPoolList[i].activeInHierarchy)
            {
                spawnPoolList[i].transform.position = position;
                spawnPoolList[i].transform.rotation = rotation;
                spawnPoolList[i].SetActive(true);
                break;
            }

            if (spawnPoolList[spawnPoolList.Count - 1].activeInHierarchy)
                AddMorePoolObjects();
        }
    }

    private void OnDisable()
    {
        isInit = false;
    }
}