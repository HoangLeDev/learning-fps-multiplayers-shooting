using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Update()
    {
        AdditionGameInputSetting();
    }

    private void Start()
    {
        //Hide Mouse cursor while playing
        Cursor.lockState = CursorLockMode.Locked;
        
        SpawnManager.I.InitSpawnManager();
    }

    private void AdditionGameInputSetting()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    } 
}
