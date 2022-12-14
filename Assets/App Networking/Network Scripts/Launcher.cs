using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Launcher : SingletonNetworking<Launcher>
{
    public GameObject menuBtns;

    [Header("Screen Fields")]
    public GameObject loadingScreen;
    public TMP_Text loadingTMP;

    public GameObject errorScreen;
    public TMP_Text errorTMP;

    [Header("Create Room")]
    public GameObject createRoomScreen;
    public TMP_InputField roomNameInput;

    [Header("In Room")]
    public GameObject roomScreen;
    public TMP_Text roomNameTMP;
    public TMP_Text roomMemberName;
    public Transform roomMemberContainer;
    private List<TMP_Text> allRoomMemberNamesList = new List<TMP_Text>();
    public string levelToPlay;
    public GameObject startPlayGameBtn;

    [Header("Browse Room")]
    public GameObject roomBrowseScreen;
    public Transform roomButtonContainer;
    public RoomButton roomButtonItem;

    private List<RoomButton> allRoomButtonsList = new List<RoomButton>();
    private List<RoomInfo> clientAvailableRoomList = new List<RoomInfo>();
    private List<RoomInfo> clientUnavailableRoomList = new List<RoomInfo>();

    [Header("Name Input Screen")]
    public GameObject nameInputScreen;
    public TMP_InputField nameInput;
    private bool isAlreadySetNickName;

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
        PhotonNetwork.AutomaticallySyncScene = true;
        loadingTMP.text = ConstantHolder.MESSAGE_JOIN_LOBBY;
    }

    public override void OnJoinedLobby()
    {
        OnBackToMainMenuBtn();
        CheckPlayerSetName();
    }

    private void CheckPlayerSetName()
    {
        if (!isAlreadySetNickName)
        {
            CloseAllPanels();
            nameInputScreen.SetActive(true);

            if (PlayerPrefs.HasKey(ConstantHolder.PP_PlayerName))
            {
                nameInput.text = PlayerPrefs.GetString(ConstantHolder.PP_PlayerName);
            }
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString(ConstantHolder.PP_PlayerName);
        }
    }

    public void OnSetNickNameBtn()
    {
        if (!string.IsNullOrEmpty(nameInput.text))
        {
            PhotonNetwork.NickName = nameInput.text;

            PlayerPrefs.SetString(ConstantHolder.PP_PlayerName, nameInput.text);

            CloseAllPanels();
            menuBtns.SetActive(true);

            isAlreadySetNickName = true;
        }
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

    #endregion

    #region IN_ROOM

    public override void OnJoinedRoom()
    {
        CloseAllPanels();
        roomScreen.SetActive(true);

        roomNameTMP.text = PhotonNetwork.CurrentRoom.Name;
        ListAllPlayerInRoom();

        CheckCanStartGameAsMasterClient();
    }

    private void ListAllPlayerInRoom()
    {
        foreach (TMP_Text mem in allRoomMemberNamesList)
        {
            Destroy(mem.gameObject);
        }

        allRoomMemberNamesList.Clear();

        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++)
        {
            TMP_Text newPlayerLable = Instantiate(roomMemberName, roomMemberContainer);
            newPlayerLable.text = players[i].NickName;
            newPlayerLable.gameObject.SetActive(true);

            allRoomMemberNamesList.Add(newPlayerLable);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newPlayerLable = Instantiate(roomMemberName, roomMemberContainer);
        newPlayerLable.text = newPlayer.NickName;
        newPlayerLable.gameObject.SetActive(true);

        allRoomMemberNamesList.Add(newPlayerLable);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayerInRoom();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        CheckCanStartGameAsMasterClient();
    }

    private void CheckCanStartGameAsMasterClient()
    {
        if (PhotonNetwork.IsMasterClient)
            startPlayGameBtn.SetActive(true);
        else startPlayGameBtn.SetActive(false);
    }

    public void OnStartPlayGameBtn()
    {
        PhotonNetwork.LoadLevel(levelToPlay);
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
        nameInputScreen.SetActive(false);
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