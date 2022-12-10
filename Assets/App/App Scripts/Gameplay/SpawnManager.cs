using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : Singleton<SpawnManager>
{
    public Transform[] spawnPoints;

    public void InitSpawnManager()
    {
        foreach (Transform spawnPos in spawnPoints)
        {
            spawnPos.gameObject.SetActive(false);
        }
    }

    public Transform GetSpawnPosition()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}