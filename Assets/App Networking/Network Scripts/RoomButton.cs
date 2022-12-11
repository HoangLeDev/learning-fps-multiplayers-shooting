using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Realtime;

public class RoomButton : MonoBehaviour
{
    public TMP_Text buttonTMP;
    private RoomInfo info;

    public void SetButtonDetails(RoomInfo inputInfo)
    {
        info = inputInfo;
        buttonTMP.text = info.Name;
    }
    private void Start()
    {
        
    }

    private void Update()
    {
    }
}
