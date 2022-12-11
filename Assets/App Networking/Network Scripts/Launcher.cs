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

    public GameObject roomBrowseScreen;
    public RoomButton roomButtonItem;
    private List<RoomButton> allRoomButtons = new List<RoomButton>();

    #region Main Function Calls

    public void Start()
    {
        CloseAllPanels();
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
        OnBackToMainMenuBtn();
    }

    public override void OnJoinedRoom()
    {
        CloseAllPanels();
        roomScreen.SetActive(true);

        roomNameTMP.text = PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorTMP.text = $"{returnCode}: {ConstantHolder.MESSAGE_CREATE_ROOM_FAILED} {message}";
        CloseAllPanels();
        errorScreen.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        OnBackToMainMenuBtn();
    }

    #endregion

    #region UI Controlling

    public void OpenRoomCreateChoiceBtn()
    {
        CloseAllPanels();
        createRoomScreen.SetActive(true);
    }

    private void CloseAllPanels()
    {
        loadingScreen.SetActive(false);
        menuBtns.SetActive(false);
        createRoomScreen.SetActive(false);
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
        roomBrowseScreen.SetActive(false);
    }

    public void OnLeaveRoomBtn()
    {
        PhotonNetwork.LeaveRoom();
        CloseAllPanels();
        loadingTMP.text = ConstantHolder.MESSAGE_LEAVING_ROOM;
        loadingScreen.SetActive(true);
    }

    public void OnOpenRoomBrowserBtn()
    {
        CloseAllPanels();
        roomBrowseScreen.SetActive(true);
    }

    public void OnBackToMainMenuBtn()
    {
        CloseAllPanels();
        menuBtns.SetActive(true);
    }

    public void OnQuitGameBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
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

            CloseAllPanels();
            loadingTMP.text = ConstantHolder.MESSAGE_CREATING_ROOM;
            loadingScreen.SetActive(true);
        }
    }

    #endregion
}