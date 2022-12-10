using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using WebSocketSharp;

public class Launcher : SingletonNetworking<Launcher>
{
    public GameObject loadingScreen;
    public GameObject menuBtns;

    public GameObject createRoomScreen;
    public TMP_InputField roomNameInput;

    public TMP_Text loadingTMP;
    public GameObject demoGun;

    #region Main Function Calls

    public void Start()
    {
        CloseMenu();
        loadingScreen.SetActive(true);
        loadingTMP.text = ConstantHolder.MESSAGE_CONNECT_TO_SERVER;

        PhotonNetwork.ConnectUsingSettings();
    }

    #endregion

    #region Photon PUN Functions

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        loadingTMP.text = ConstantHolder.MESSAGE_JOIN_LOBBY;
    }

    public override void OnJoinedLobby()
    {
        CloseMenu();
        menuBtns.SetActive(true);
    }

    #endregion

    #region UI Controlling

    public void OpenRoomCreateChoice()
    {
        CloseMenu();
        createRoomScreen.SetActive(true);
    }

    private void CloseMenu()
    {
        loadingScreen.SetActive(false);
        menuBtns.SetActive(false);
        createRoomScreen.SetActive(false);
    }

    #endregion

    #region Methods

    public void StartCreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 8;

            PhotonNetwork.CreateRoom(roomNameInput.text, options);

            CloseMenu();
            loadingTMP.text = ConstantHolder.MESSAGE_CREATING_ROOM;
            loadingScreen.SetActive(true);
        }
    }

    #endregion
}