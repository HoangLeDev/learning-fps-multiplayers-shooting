using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

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
    public Transform roomButtonContainer;
    public RoomButton roomButtonItem;
    public List<RoomButton> allRoomButtonsList = new List<RoomButton>();
    public List<RoomInfo> clientAvailableRoomList = new List<RoomInfo>();
    public List<RoomInfo> clientUnavailableRoomList = new List<RoomInfo>();

    #region Main Function Calls

    public void Start()
    {
        CloseAllPanels();
        loadingScreen.SetActive(true);
        loadingTMP.text = ConstantHolder.MESSAGE_CONNECT_TO_SERVER;

        PhotonNetwork.ConnectUsingSettings();
    }

    #endregion


    #region LOBBY

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        loadingTMP.text = ConstantHolder.MESSAGE_JOIN_LOBBY;
    }

    public override void OnJoinedLobby()
    {
        OnBackToMainMenuBtn();
    }

    #endregion


    #region CREATE_ROOM

    public void OpenRoomCreateChoiceBtn()
    {
        CloseAllPanels();
        createRoomScreen.SetActive(true);
    }

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

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorTMP.text = $"{returnCode}: {ConstantHolder.MESSAGE_CREATE_ROOM_FAILED} {message}";
        CloseAllPanels();
        errorScreen.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        CloseAllPanels();
        roomScreen.SetActive(true);

        roomNameTMP.text = PhotonNetwork.CurrentRoom.Name;
    }

    #endregion

    #region LEAVE_ROOM

    public void OnLeaveRoomBtn()
    {
        PhotonNetwork.LeaveRoom();
        CloseAllPanels();
        loadingTMP.text = ConstantHolder.MESSAGE_LEAVING_ROOM;
        loadingScreen.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        OnBackToMainMenuBtn();
    }

    #endregion

    #region BROWSE_ROOM

    public void OnOpenRoomBrowserBtn()
    {
        CloseAllPanels();
        roomBrowseScreen.SetActive(true);
    }


    public void OnJoinRoomAfterBrowseBtn(RoomInfo inputInfo)
    {
        PhotonNetwork.JoinRoom(inputInfo.Name);

        CloseAllPanels();
        loadingTMP.text = ConstantHolder.MESSAGE_JOIN_ROOM;
        loadingScreen.SetActive(true);
    }

    #endregion

    #region PHOTON_UPDATE_ROOM

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomStatusHandler(clientAvailableRoomList, clientUnavailableRoomList, roomList);
        for (int i = 0; i < allRoomButtonsList.Count; i++)
        {
            Destroy(allRoomButtonsList[i].gameObject);
        }

        allRoomButtonsList.Clear();

        for (int i = 0; i < clientAvailableRoomList.Count; i++)
        {
            if (clientAvailableRoomList[i].PlayerCount != clientAvailableRoomList[i].MaxPlayers)
            {
                RoomButton newButton = Instantiate(roomButtonItem, roomButtonContainer);
                newButton.SetButtonDetails(clientAvailableRoomList[i]);
                newButton.gameObject.SetActive(true);

                allRoomButtonsList.Add(newButton);
            }
        }
    }

    private void RoomStatusHandler(List<RoomInfo> clientAvailableRoomList, List<RoomInfo> clientUnavailableRoomList,
        List<RoomInfo> ServerRoomList)
    {
        foreach (var rb in ServerRoomList)
        {
            if (!rb.RemovedFromList)
            {
                if (!clientAvailableRoomList.Contains(rb))
                    clientAvailableRoomList.Add(rb);
            }
            else
            {
                clientUnavailableRoomList.Add(rb);
            }
        }

        foreach (var rb in clientUnavailableRoomList)
        {
            if (clientAvailableRoomList.Contains(rb))
                clientAvailableRoomList.Remove(rb);
        }
    }

    #endregion

    #region METHODS

    public void OnBackToMainMenuBtn()
    {
        CloseAllPanels();
        menuBtns.SetActive(true);
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

    public void OnQuitGameBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    #endregion
}