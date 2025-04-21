using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameLoopManager : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class TurnChangeEvent : UnityEvent<int> { }

    public float turnDuration = 15f;
    public int currentPlayerIndex = 0;
    private bool isGameRunning = true;

    public Category currentCategory;

    public TurnChangeEvent OnTurnChanged;
    public UnityEvent<int> OnTurnStart;
    public UnityEvent<int> OnTurnEnd;

    private Coroutine turnCoroutine;

    // For Editor Only

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartGameLoop());
        }
    }

    IEnumerator StartGameLoop()
    {
        yield return new WaitForSeconds(1f); // slight delay for setup
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (isGameRunning)
        {
            int actorNumber = PhotonNetwork.PlayerList[currentPlayerIndex].ActorNumber;

            photonView.RPC(nameof(RPC_StartTurn), RpcTarget.All, actorNumber, (int)currentCategory);

            yield return new WaitForSeconds(turnDuration);

            photonView.RPC(nameof(RPC_EndTurn), RpcTarget.All, actorNumber);

            currentPlayerIndex = (currentPlayerIndex + 1) % PhotonNetwork.PlayerList.Length;

            ChangeCategory();
        }
    }

    [PunRPC]
    void RPC_StartTurn(int actorNumber, int category)
    {
        currentCategory = (Category)category;
        OnTurnStart?.Invoke(actorNumber);
        OnTurnChanged?.Invoke(actorNumber);
    }

    [PunRPC]
    void RPC_EndTurn(int actorNumber)
    {
        OnTurnEnd?.Invoke(actorNumber);
    }

    private void ChangeCategory()
    {
        Category[] categories = {
            Category.Architectural,
            Category.Electrical,
            Category.Plumbing
        };

        currentCategory = categories[Random.Range(0, categories.Length)];
        photonView.RPC(nameof(RPC_UpdateCategory), RpcTarget.All, (int)currentCategory);
    }

    [PunRPC]
    void RPC_UpdateCategory(int newCategory)
    {
        currentCategory = (Category)newCategory;
        Debug.Log($"New Category: {currentCategory}");
    }

    public bool CanPlayCategory(CardType cardType)
    {
        switch (currentCategory)
        {
            case Category.Architectural:
                return cardType == CardType.Architectural;
            case Category.Electrical:
                return cardType == CardType.Electrical;
            case Category.Plumbing:
                return cardType == CardType.Plumbing;
            default:
                return false;
        }
    }
}