using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonLobbyUIManager : MonoBehaviourPunCallbacks
{
    public Dictionary<string, GameObject> playerIcons = new Dictionary<string, GameObject>();

    public GameObject MainCanvas, VSCanvas, ClassicCanvas, MatchIDCanvas;

    public GameObject playerIconPrefab;
    public Transform iconParent;

    [Header("Buttons")]
    public Button startGameButton;
    public Button quitRoomButton;

    public int currentPlayerIndex;

    // Assuming you have a list of predefined sprites based on character selection or random
    public Sprite[] possibleAvatars;

    public GameObject[] placeholders;

    private void Start()
    {
        quitRoomButton.onClick.AddListener(LeaveRoom);
        currentPlayerIndex = 0;
    }

    public void InitializePlayerIcon()
    {
        MatchIDCanvas.SetActive(false);
        VSCanvas.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;

        // Random avatar for this example
        int avatarIndex = PlayerPrefs.HasKey("ProfileAvatar") ? PlayerPrefs.GetInt("ProfileAvatar") : 0;
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

        placeholders[currentPlayerIndex].SetActive(false);
        currentPlayerIndex++;

        foreach (GameObject obj in placeholders)
        {
            if (obj != null && obj.transform.parent == iconParent)
            {
                obj.transform.SetAsLastSibling();
            }
        }

        Debug.Log($"{playerName} has joined.");

        // Store this player icon in the dictionary
        playerIcons[playerName] = iconObj;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();

        // Destroy all GameObjects in the dictionary
        foreach (var kvp in playerIcons)
        {
            if (kvp.Value != null)
            {
                GameObject.Destroy(kvp.Value);
            }
        }

        // Clear the dictionary after destroying the GameObjects
        playerIcons.Clear();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        Debug.Log("You have left the room.");

        VSCanvas.SetActive(false);
        MainCanvas.SetActive(true);

        foreach(var go in placeholders)
        {
            go.SetActive(true);
        }
        currentPlayerIndex = 0;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        Debug.Log("A Player has left the room.");

        // Remove the player from the dictionary and destroy the icon
        if (playerIcons.ContainsKey(otherPlayer.NickName))
        {
            GameObject iconToDestroy = playerIcons[otherPlayer.NickName];
            Destroy(iconToDestroy);
            playerIcons.Remove(otherPlayer.NickName);
        }

        currentPlayerIndex--;
        placeholders[currentPlayerIndex].SetActive(true);

        foreach (GameObject obj in placeholders)
        {
            if (obj != null && obj.transform.parent == iconParent)
            {
                obj.transform.SetAsLastSibling();
            }
        }
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
        photonView.RPC("RPC_RemovePlayerIcon", RpcTarget.AllBuffered, PhotonNetwork.NickName);
    }

    [PunRPC]
    void RPC_RemovePlayerIcon(string playerName)
    {
        Debug.Log("Player left the room.");

        // Destroy the player icon for the player who left
        if (playerIcons.ContainsKey(playerName))
        {
            GameObject iconToDestroy = playerIcons[playerName];
            Destroy(iconToDestroy);
            playerIcons.Remove(playerName);
        }
    }

    public void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
    }
}
