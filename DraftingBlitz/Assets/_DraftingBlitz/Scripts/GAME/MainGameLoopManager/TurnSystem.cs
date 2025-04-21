using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ExitGames.Client.Photon;

public class TurnSystem : MonoBehaviourPunCallbacks, IPunObservable
{
    public static TurnSystem Instance;

    public int currentTurn = 0;
    private const int MaxPlayers = 3;
    private int localPlayerId;
    public bool isYourTurn = false;

    [Header("UI")]
    public TMP_Text turnText;
    public TMP_Text categoryText;

    public Category currentCategory;

    [Header("References")]
    public CardManager cardManager;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        localPlayerId = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        cardManager.cardHands[0].identifier = localPlayerId;

        switch(localPlayerId)
        {
            case 0:
                {
                    cardManager.cardHands[1].identifier = 1;
                    cardManager.cardHands[2].identifier = 2;
                    break;
                }
            case 1:
                {
                    cardManager.cardHands[1].identifier = 0;
                    cardManager.cardHands[2].identifier = 2;
                    break;
                }
            case 2:
                {
                    cardManager.cardHands[1].identifier = 0;
                    cardManager.cardHands[2].identifier = 1;
                    break;
                }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            Category newCategory = ShuffleCategory();
            photonView.RPC("RPC_SetCategory", RpcTarget.AllBuffered, (int)newCategory);
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f); // Allow scene to load

        SetPlayerReady(); // Signal that you're ready

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(WaitForAllPlayersReady());
        }
    }

    void SetPlayerReady()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "ready", true }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    bool AllPlayersReady()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("ready", out object isReady))
            {
                if (!(bool)isReady)
                    return false;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator WaitForAllPlayersReady()
    {
        while (!AllPlayersReady())
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f); // Optional buffer
        SetupHands();
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (changedProps.ContainsKey("ready") && AllPlayersReady())
        {
            StartCoroutine(WaitForAllPlayersReady());
        }
    }

    void SetupHands()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        int localId = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        for (int i = 0; i < playerCount; i++)
        {
            if (cardManager.cardHands[i].cards.Count > 0)
                continue;

            bool isOwn = (i == localId);
            int relativeIndex = (i - localId + playerCount) % playerCount;

            CardHand.HandType type = CardHand.HandType.Bottom;

            if (relativeIndex == 0)
                type = CardHand.HandType.Bottom;
            else if (relativeIndex == 1)
                type = CardHand.HandType.Left;
            else if (relativeIndex == 2)
                type = CardHand.HandType.Right;

            photonView.RPC("RPC_CreateHand", RpcTarget.All, i, (int)type, isOwn);
        }
    }

    [PunRPC]
    void RPC_CreateHand(int playerIndex, int handType, bool ownHand)
    {
        cardManager.GenerateHand(playerIndex, (CardHand.HandType)handType, ownHand);
    }

    void Update()
    {
        isYourTurn = (localPlayerId == currentTurn);
        turnText.text = isYourTurn ? "Your Turn" : "Other's Turn";
    }

    public void EndTurn(int cardIndex)
    {
        if (!isYourTurn) return;

        currentTurn = (currentTurn + 1) % PhotonNetwork.CurrentRoom.PlayerCount;
        photonView.RPC("RPC_UpdateTurn", RpcTarget.AllBuffered, currentTurn);
        photonView.RPC("RPC_RemoveCard", RpcTarget.AllBuffered, localPlayerId, cardIndex);
    }

    [PunRPC]
    void RPC_UpdateTurn(int newTurn)
    {
        currentTurn = newTurn;
    }

    [PunRPC]
    void RPC_RemoveCard(int playerId, int cardIndex)
    {
        foreach(var cardHands in cardManager.cardHands)
        {
            if(cardHands.identifier == playerId)
            {
                cardHands.RemoveCard(cardIndex);
            }
        }
    }

    [PunRPC]
    void RPC_SetCategory(int categoryIndex)
    {
        currentCategory = (Category)categoryIndex;
        categoryText.text = $"Category\n{currentCategory}";
    }

    public void PlayCard(int cardIndex)
    {
        if (!isYourTurn) return;

        Category newCategory = ShuffleCategory();
        photonView.RPC("RPC_SetCategory", RpcTarget.AllBuffered, (int)newCategory);

        EndTurn(cardIndex);
    }

    public Category ShuffleCategory()
    {
        int randomIndex = Random.Range(0, System.Enum.GetValues(typeof(Category)).Length);
        return (Category)randomIndex;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentTurn);
            stream.SendNext((int)currentCategory);
        }
        else
        {
            currentTurn = (int)stream.ReceiveNext();
            currentCategory = (Category)stream.ReceiveNext();
        }
    }
}
