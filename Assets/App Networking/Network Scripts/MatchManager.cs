using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class MatchManager : Singleton<MatchManager>
{
    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
            SceneManager.LoadScene("Main Menu");
    }
}