using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Launcher : SingletonNetworking<Launcher>
{
    public GameObject loadingScreen;
    public GameObject menuBtns;
    public TMP_Text loadingTMP;

    public void Start()
    {
        CloseMenu();
    }
    
    private void CloseMenu()
    {
        loadingScreen.SetActive(false);
        menuBtns.SetActive(false);
    }
}
