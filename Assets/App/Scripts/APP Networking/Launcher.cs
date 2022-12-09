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
        loadingScreen.SetActive(true);
        loadingTMP.text = ConstantHolder.MESSAGE_CONNECT_TO_SERVER + "...";
    }
    
    private void CloseMenu()
    {
        loadingScreen.SetActive(false);
        menuBtns.SetActive(false);
    }
}
