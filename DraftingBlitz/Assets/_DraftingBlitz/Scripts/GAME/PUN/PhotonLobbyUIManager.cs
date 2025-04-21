using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonLobbyUIManager : MonoBehaviourPunCallbacks
{
    public Dictionary<string, GameObject> playerIcons = new Dictionary<string, GameObject>();


    public GameObject playerIconPrefab;
    public Transform iconParent;
    public Button startGameButton;

    // Assuming you have a list of predefined sprites based on character selection or random
    public Sprite[] possibleAvatars;

    public void InitializePlayerIcon()
    {
        if (PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;

        // Random avatar for this example
        int avatarIndex = Random.Range(0, possibleAvatars.Length);
        string playerName = PhotonNetwork.NickName;

        Debug.Log("Player Joined");

        // Tell everyone to add this player
        photonView.RPC(nameof(RPC_AddPlayerIcon), RpcTarget.AllBuffered, playerName, avatarIndex);
    }

    [PunRPC]
    void RPC_AddPlayerIcon(string playerName, int avatarIndex)
    {
        GameObject iconObj = Instantiate(playerIconPrefab, iconParent);
        PlayerIconUI icon = iconObj.GetComponent<PlayerIconUI>();

        Sprite avatar = possibleAvatars[avatarIndex];
        icon.Setup(avatar, playerName);

        Debug.Log($"{playerName} has joined.");

        // Store this player icon in the dictionary
        playerIcons[playerName] = iconObj;
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        Debug.Log("Player left the room.");

        // Remove the player from the dictionary and destroy the icon
        if (playerIcons.ContainsKey(PhotonNetwork.NickName))
        {
            GameObject iconToDestroy = playerIcons[PhotonNetwork.NickName];
            Destroy(iconToDestroy);
            playerIcons.Remove(PhotonNetwork.NickName);
        }

        // Notify all other players to destroy the icon of the player who left
        photonView.RPC(nameof(RPC_RemovePlayerIcon), RpcTarget.AllBuffered, PhotonNetwork.NickName);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Player disconnected from the room.");

        // Remove the player from the dictionary and destroy the icon
        if (playerIcons.ContainsKey(PhotonNetwork.NickName))
        {
            GameObject iconToDestroy = playerIcons[PhotonNetwork.NickName];
            Destroy(iconToDestroy);
            playerIcons.Remove(PhotonNetwork.NickName);
        }

        // Notify all other players to destroy the icon of the player who left
        photonView.RPC(nameof(RPC_RemovePlayerIcon), RpcTarget.AllBuffered, PhotonNetwork.NickName);
    }

    [PunRPC]
    void RPC_RemovePlayerIcon(string playerName)
    {
        // Destroy the player icon for the player who left
        if (playerIcons.ContainsKey(playerName))
        {
            GameObject iconToDestroy = playerIcons[playerName];
            Destroy(iconToDestroy);
            playerIcons.Remove(playerName);
        }
    }
}
