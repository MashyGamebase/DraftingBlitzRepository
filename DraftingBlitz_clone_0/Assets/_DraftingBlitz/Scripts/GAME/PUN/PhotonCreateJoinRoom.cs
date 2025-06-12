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

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void CreateRoom(string roomName)
    {
        if (PhotonNetwork.IsConnected)
        {
            RoomOptions options = new RoomOptions
            {
                MaxPlayers = 4,
                IsVisible = true,
                IsOpen = true
            };

            PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
        }
        else
        {
            Debug.LogWarning("Not connected to Photon.");
        }
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

        PhotonNetwork.LoadLevel("GameScene");
    }

    [PunRPC]
    void RPC_StartTransition()
    {
        transtionObject.SetActive(true);
        transitionPlayer.Play();
    }

    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    // Called when connected to Photon master server
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server.");
        PhotonNetwork.JoinLobby(); // optional but good for listing rooms
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created. You are the Master Client.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");

        GetComponent<PhotonLobbyUIManager>().InitializePlayerIcon();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Join failed: {message}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Create failed: {message}");
    }
}
