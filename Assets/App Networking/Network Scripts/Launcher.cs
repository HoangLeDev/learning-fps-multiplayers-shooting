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
    public GameObject menuBtns;

    [Header("Screen Fields")]
    //Screen Fields
    public GameObject loadingScreen;

    public TMP_Text loadingTMP;

    public GameObject createRoomScreen;
    public TMP_InputField roomNameInput;

    public GameObject roomScreen;
    public TMP_Text roomNameTMP;

    public GameObject errorScreen;
    public TMP_Text errorTMP;


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

    public override void OnJoinedRoom()
    {
        CloseMenu();
        roomScreen.SetActive(true);

        roomNameTMP.text = PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorTMP.text = $"{returnCode}: {ConstantHolder.MESSAGE_CREATE_ROOM_FAILED} {message}";
        CloseMenu();
        errorScreen.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        CloseMenu();
        menuBtns.SetActive(true);
    }

    #endregion

    #region UI Controlling

    public void OpenRoomCreateChoiceBtn()
    {
        CloseMenu();
        createRoomScreen.SetActive(true);
    }

    private void CloseMenu()
    {
        loadingScreen.SetActive(false);
        menuBtns.SetActive(false);
        createRoomScreen.SetActive(false);
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
    }

    public void OnCloseErrorScreenBtn()
    {
        CloseMenu();
        menuBtns.SetActive(true);
    }

    public void OnLeaveRoomBtn()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenu();
        loadingTMP.text = ConstantHolder.MESSAGE_LEAVING_ROOM;
        loadingScreen.SetActive(true);
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