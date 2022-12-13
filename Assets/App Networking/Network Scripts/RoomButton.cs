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
    public string roomName;

    public void SetButtonDetails(RoomInfo inputInfo)
    {
        info = inputInfo;
        roomName = info.Name;
        buttonTMP.text = roomName;
        transform.name = "Room: " + roomName;
    }

    public void OnOpenRoom()
    {
        Launcher.I.OnJoinRoomAfterBrowseBtn(info);
    }
}