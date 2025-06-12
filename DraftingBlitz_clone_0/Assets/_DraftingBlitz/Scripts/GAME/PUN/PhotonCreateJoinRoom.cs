using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PhotonCreateJoinRoom : MonoBehaviourPunCallbacks
{
    public GameObject transtionObject;
    public VideoPlayer transitionPlayer;

    private string targetRoomName;
    private bool isWaitingForRoomCheck = false;
    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void TryJoinOrCreateRoom(string roomName)
    {
        if (PhotonNetwork.IsConnected)
        {
            targetRoomName = roomName;
            isWaitingForRoomCheck = true;

            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby(); // will trigger OnRoomListUpdate
            }
            else
            {
                CheckRoomListForRoom(cachedRoomList); // Use latest cached list
            }
        }
        else
        {
            Debug.LogWarning("Not connected to Photon.");
        }
    }

    void CheckRoomListForRoom(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.Name == targetRoomName)
            {
                if (room.CustomProperties.TryGetValue("gameStarted", out object startedObj) && startedObj is bool started && started)
                {
                    PopupController.Instance.PopupNotif("Game already started.", 1.5f);
                    return;
                }

                PhotonNetwork.JoinRoom(targetRoomName);
                return;
            }
        }

        // Room not found, create it
        CreateRoom(targetRoomName);
    }

    public void CreateRoom(string roomName)
    {
        ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable
        {
            { "gameStarted", false }
        };

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = customProps,
            CustomRoomPropertiesForLobby = new string[] { "gameStarted" }
        };

        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartGameCO());
        }
        else
        {
            PopupController.Instance.PopupNotif("Only the host can start the game", 1f);
        }
    }

    IEnumerator StartGameCO()
    {
        photonView.RPC("RPC_StartTransition", RpcTarget.AllBuffered);

        yield return new WaitForSeconds((float)transitionPlayer.clip.length);

        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "gameStarted", true }
        });

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("GameScene");
    }

    [PunRPC]
    void RPC_StartTransition()
    {
        transtionObject.SetActive(true);
        transitionPlayer.Play();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (!isWaitingForRoomCheck) return;

        isWaitingForRoomCheck = false;
        CheckRoomListForRoom(roomList);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created. You are the Master Client.");
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameStarted", out object startedObj) &&
            startedObj is bool started && started)
        {
            PopupController.Instance.PopupNotif("Game already started.", 1.5f);
            GetComponent<PhotonLobbyUIManager>().LeaveRoom();
        }
        else
        {
            Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
            GetComponent<PhotonLobbyUIManager>().InitializePlayerIcon();
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Join failed: {message}");
        PopupController.Instance.PopupNotif("Game already started.", 1.5f);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Create failed: {message}");

        if (returnCode == ErrorCode.GameIdAlreadyExists)
        {
            PhotonNetwork.JoinRoom(targetRoomName); // fallback in case race condition
        }
    }
}
