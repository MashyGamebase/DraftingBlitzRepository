using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ExitGames.Client.Photon;
using DG.Tweening;
using System;
using UnityEngine.UI;
using System.Linq; // For sorting

public class TurnSystem : MonoBehaviourPunCallbacks, IPunObservable
{
    public static TurnSystem Instance;

    public int currentTurn = 0;
    private const int MaxPlayers = 3;
    internal int localPlayerId;
    public bool isYourTurn = false; // True or False

    public bool GameStarted = false;

    private bool isReversed = false;
    private int blockedPlayer = -1;

    public int totalCardsPlayed;

    [Header("UI")]
    public GameObject yourTurnImage;
    public TMP_Text categoryText;
    public TMP_Text timerText;

    public TMP_Text player0Score, player0Name;
    public TMP_Text player1Score, player1Name;
    public TMP_Text player2Score, player2Name;

    public Image loopIndicator;
    public Sprite normal, reversed;

    public Category currentCategory;

    [Header("References")]
    public CardManager cardManager;
    public CanvasGroup loaderBlocker;

    [Header("Sprites")]
    public Sprite architectureSprite;
    public Sprite electricalSprite;
    public Sprite plumbingSprite;
    public Sprite architectureBorderSprite;
    public Sprite electricalBorderSprite;
    public Sprite plumbingBorderSprite;

    public Image categoryImage;
    public Image borderImage;

    public GameObject startPanel;
    public Sprite[] startSprites;
    public Image startImage;

    [Header("Turn Cooldown")]
    public Image cooldownRadialImage; // Radial 360 fill image
    public float turnCooldown = 8f;
    private float currentCooldown = 0f;
    private bool isCooldownActive = false;

    [Header("Audio")]
    public AudioSource sfxSource;

    public AudioClip dealCardsClip, flipCardClip, pointsObtainedClip, loseATurnClip;

    // Timer
    public float categoryTimer = 15f;

    // Player Scores
    public Dictionary<int, float> playerScores = new Dictionary<int, float>();
    [SerializeField] private List<string> debugPlayerScores = new List<string>();

    // Player UI
    public PlayerIconSetter playerIconSetter;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        localPlayerId = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        cardManager.cardHands[0].identifier = localPlayerId;

        switch (localPlayerId)
        {
            case 0:
                cardManager.cardHands[1].identifier = 1;
                cardManager.cardHands[2].identifier = 2;
                break;
            case 1:
                cardManager.cardHands[1].identifier = 0;
                cardManager.cardHands[2].identifier = 2;
                break;
            case 2:
                cardManager.cardHands[1].identifier = 0;
                cardManager.cardHands[2].identifier = 1;
                break;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            SwitchCategory();
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        loaderBlocker.DOFade(0, 0.5f).OnComplete(() =>
        {
            loaderBlocker.gameObject.SetActive(false);
            SetPlayerReady();
        });

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(WaitForAllPlayersReady());
        }
    }

    public void FlipCard()
    {
        sfxSource.PlayOneShot(flipCardClip);
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
                if (!(bool)isReady) return false;
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

        yield return new WaitForSeconds(0.5f);
        SetupHands();
    }

    void InitializePlayerScores()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            int playerId = player.ActorNumber - 1;
            if (!playerScores.ContainsKey(playerId))
            {
                playerScores.Add(playerId, 0f);
            }
        }
    }

    void RefreshDebugScores()
    {
        debugPlayerScores.Clear();
        foreach (var kvp in playerScores)
        {
            debugPlayerScores.Add($"Player {kvp.Key}: {kvp.Value} points");
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (changedProps.ContainsKey("ready") && AllPlayersReady())
        {
            StartCoroutine(WaitForAllPlayersReady());
            InitializePlayerScores();
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
            if (relativeIndex == 1) type = CardHand.HandType.Left;
            else if (relativeIndex == 2) type = CardHand.HandType.Right;

            photonView.RPC("RPC_CreateHand", RpcTarget.All, i, (int)type, isOwn, PhotonNetwork.NickName, PlayerPrefs.GetInt("ProfileAvatar"));
        }
    }

    [PunRPC]
    void RPC_CreateHand(int playerIndex, int handType, bool ownHand, string playerName, int playerIconIndex)
    {
        startImage.gameObject.SetActive(true);
        StartCoroutine(StartSequenceCO(playerIndex, handType, ownHand));

        playerIconSetter.SetNameUI(playerIndex, playerName, playerIconIndex);
    }

    IEnumerator StartSequenceCO(int playerIndex, int handType, bool ownHand)
    {
        startImage.sprite = startSprites[0];
        yield return new WaitForSeconds(1f);
        startImage.sprite = startSprites[1];
        yield return new WaitForSeconds(1f);
        startImage.sprite = startSprites[2];
        yield return new WaitForSeconds(1f);

        if (PhotonNetwork.IsMasterClient)
            GameStarted = true;

        startPanel.SetActive(false);

        if (!sfxSource.isPlaying)
            sfxSource.PlayOneShot(dealCardsClip);

        cardManager.GenerateHand(playerIndex, (CardHand.HandType)handType, ownHand);
    }

    void Update()
    {
        isYourTurn = (localPlayerId == currentTurn);
        yourTurnImage.SetActive(isYourTurn);

        loopIndicator.sprite = isReversed ? reversed : normal;

#if UNITY_EDITOR
        RefreshDebugScores();
#endif

        if (PhotonNetwork.IsMasterClient && GameStarted)
        {
            categoryTimer -= Time.deltaTime;

            if (categoryTimer <= 0f)
            {
                SwitchCategory();
            }

            // Handle player turn cooldown
            if (isYourTurn)
            {
                if (!isCooldownActive)
                {
                    currentCooldown = turnCooldown;
                    cooldownRadialImage.fillAmount = 1f;
                    isCooldownActive = true;
                }

                currentCooldown -= Time.deltaTime;
                float t = 1f - Mathf.Clamp01(currentCooldown / turnCooldown);

                cooldownRadialImage.fillAmount = 1f - t;
                cooldownRadialImage.color = Color.Lerp(Color.green, Color.red, t);

                if (currentCooldown <= 0f)
                {
                    SkipTurn(); // Automatically skip turn when cooldown ends
                    ResetCooldown();
                }
            }
        }

        timerText.text = $"CATEGORY CHANGES IN...{Mathf.CeilToInt(categoryTimer)}s";

        UpdateScoreUI(); // === Added: Update score display ordered by score
    }

    void ResetCooldown()
    {
        isCooldownActive = false;
        currentCooldown = 0f;
        cooldownRadialImage.fillAmount = 1f;
        cooldownRadialImage.color = Color.green;
    }

    // === Added: Method to update the UI scores and names, ordered by score descending
    void UpdateScoreUI()
    {
        // Build a list of tuples (playerId, score, name)
        var playersData = new List<(int playerId, float score, string name)>();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            int id = player.ActorNumber - 1;
            float score = playerScores.ContainsKey(id) ? playerScores[id] : 0f;
            string name = player.NickName;
            playersData.Add((id, score, name));
        }

        // Sort descending by score
        playersData = playersData.OrderByDescending(p => p.score).ToList();

        // Assign sorted players to UI slots
        if (playersData.Count > 0)
        {
            player0Name.text = playersData[0].name;
            player0Score.text = playersData[0].score.ToString("F0");
        }
        else
        {
            player0Name.text = "N/A";
            player0Score.text = "0";
        }

        if (playersData.Count > 1)
        {
            player1Name.text = playersData[1].name;
            player1Score.text = playersData[1].score.ToString("F0");
        }
        else
        {
            player1Name.text = "N/A";
            player1Score.text = "0";
        }

        if (playersData.Count > 2)
        {
            player2Name.text = playersData[2].name;
            player2Score.text = playersData[2].score.ToString("F0");
        }
        else
        {
            player2Name.text = "N/A";
            player2Score.text = "0";
        }
    }

    public void EndTurn(int cardIndex, string cardName, string category)
    {
        if (!isYourTurn) return;

        currentTurn = (currentTurn + (isReversed ? -1 : 1) + PhotonNetwork.CurrentRoom.PlayerCount) % PhotonNetwork.CurrentRoom.PlayerCount;
        ResetCooldown();
        photonView.RPC("RPC_UpdateTurn", RpcTarget.AllBuffered, currentTurn);
        photonView.RPC("RPC_RemoveCard", RpcTarget.AllBuffered, localPlayerId, cardIndex, cardName, category);
    }

    public void SkipTurn()
    {
        currentTurn = (currentTurn + (isReversed ? -1 : 1) + PhotonNetwork.CurrentRoom.PlayerCount) % PhotonNetwork.CurrentRoom.PlayerCount;
        photonView.RPC("RPC_UpdateTurn", RpcTarget.AllBuffered, currentTurn);
        ResetCooldown();
    }

    public void DrawCard()
    {
        if (!isYourTurn) return;

        foreach (var cardHands in cardManager.cardHands)
        {
            if (cardHands.identifier == localPlayerId)
            {
                if (cardHands.cards.Count >= cardManager.deckLimit) // Prevets losing a turn and drawing more than 7 cards
                    return;
            }
        }

        currentTurn = (currentTurn + (isReversed ? -1 : 1) + PhotonNetwork.CurrentRoom.PlayerCount) % PhotonNetwork.CurrentRoom.PlayerCount;
        photonView.RPC("RPC_AnimateDrawCard", RpcTarget.AllBuffered, localPlayerId);
        photonView.RPC("RPC_UpdateTurn", RpcTarget.AllBuffered, currentTurn);

        ResetCooldown();
        // No category switch here anymore
    }

    public void SwitchCategory()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        categoryTimer = 15f;

        int categoryCount = Enum.GetValues(typeof(Category)).Length;
        int randomIndex = UnityEngine.Random.Range(0, categoryCount);
        photonView.RPC("RPC_SetCategory", RpcTarget.AllBuffered, randomIndex);
    }

    [PunRPC]
    void RPC_SetCategory(int categoryIndex)
    {
        currentCategory = (Category)categoryIndex;
        categoryText.text = $"Category\n{currentCategory}";

        switch (currentCategory)
        {
            case Category.Architectural:
                categoryImage.sprite = architectureSprite;
                borderImage.sprite = architectureBorderSprite;
                break;
            case Category.Electrical:
                categoryImage.sprite = electricalSprite;
                borderImage.sprite = electricalBorderSprite;
                break;
            case Category.Plumbing:
                categoryImage.sprite = plumbingSprite;
                borderImage.sprite = plumbingBorderSprite;
                break;
        }
    }

    public void PlayCard(int cardIndex, string cardName, CardType cardCategory)
    {
        if (!isYourTurn) return;

        if (cardCategory == CardType.Action)
        {
            if (cardName.Contains("Block"))
            {
                photonView.RPC("RPC_BlockNextPlayer", RpcTarget.AllBuffered, localPlayerId);
            }
            else if (cardName.Contains("Delete"))
            {
                photonView.RPC("RPC_RequestDelete", RpcTarget.AllBuffered, localPlayerId);
            }
            else if (cardName.Contains("Rotate"))
            {
                photonView.RPC("RPC_ReverseTurnOrder", RpcTarget.AllBuffered);
            }

            photonView.RPC("RPC_RemoveCard", RpcTarget.AllBuffered, localPlayerId, cardIndex, cardName, "Action");
            EndTurn(cardIndex, cardName, "Action");
        }
        else if (cardCategory == CardType.Special)
        {
            if (cardName.Contains("Scale"))
            {
                photonView.RPC("RPC_ScaleScore", RpcTarget.AllBuffered, localPlayerId);
            }
            else if (cardName.Contains("Divide"))
            {
                int nextPlayerId = (currentTurn + (isReversed ? -1 : 1) + PhotonNetwork.CurrentRoom.PlayerCount) % PhotonNetwork.CurrentRoom.PlayerCount;
                photonView.RPC("RPC_DivideScore", RpcTarget.AllBuffered, nextPlayerId);
            }
            else if (cardName.Contains("Erase"))
            {
                photonView.RPC("RPC_EraseLastPiece", RpcTarget.AllBuffered);
            }

            EndTurn(cardIndex, cardName, "Special");
            photonView.RPC("RPC_RemoveCard", RpcTarget.AllBuffered, localPlayerId, cardIndex, cardName, "Special");
        }
        else if (currentCategory.ToString() == cardCategory.ToString())
        {
            string playedCategory = currentCategory.ToString();
            totalCardsPlayed++;
            EndTurn(cardIndex, cardName, playedCategory);
        }
    }

    [PunRPC]
    void RPC_AnimateDrawCard(int playerID)
    {
        foreach (var cardHands in cardManager.cardHands)
        {
            if (cardHands.identifier == playerID)
            {
                Card card = cardManager.cardDatabase.GetRandomCard();
                cardHands.AddCard(card.CardVisual, card);
            }
        }
    }

    [PunRPC]
    void RPC_UpdateTurn(int newTurn)
    {
        if (blockedPlayer == newTurn)
        {
            blockedPlayer = -1; // reset
            newTurn = (newTurn + (isReversed ? -1 : 1) + PhotonNetwork.CurrentRoom.PlayerCount) % PhotonNetwork.CurrentRoom.PlayerCount;
        }

        currentTurn = newTurn;

        if (PhotonNetwork.IsMasterClient)
            SwitchCategory();
    }

    [PunRPC]
    void RPC_RemoveCard(int playerId, int cardIndex, string cardName, string category)
    {
        Card card = cardManager.cardDatabase.GetCardByName(cardName);

        foreach (var cardHand in cardManager.cardHands)
        {
            if (cardHand.identifier == playerId)
            {
                playerScores[playerId] += card.Points;
                cardHand.RemoveCard(cardIndex, card.CardVisual);
                FloorplanMapper.Instance.ApplyPiece(cardName, category);

                sfxSource.PlayOneShot(pointsObtainedClip);

                FloorplanMapper.Instance.lastAppliedPoints = card.Points;
            }
        }
    }

    [PunRPC]
    void RPC_BlockNextPlayer(int playerId)
    {
        blockedPlayer = (currentTurn + (isReversed ? -1 : 1) + PhotonNetwork.CurrentRoom.PlayerCount) % PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log($"Player {blockedPlayer} is blocked next round!");
    }

    [PunRPC]
    void RPC_ReverseTurnOrder()
    {
        isReversed = !isReversed;
        Debug.Log("Turn order reversed!");
    }

    [PunRPC]
    void RPC_RequestDelete(int playerId)
    {
        FloorplanMapper.Instance.RequestDeletePiece(playerId);
    }

    [PunRPC]
    public void RPC_ScaleScore(int playerId)
    {
        if (playerScores.ContainsKey(playerId))
        {
            playerScores[playerId] *= 2;
        }
    }

    [PunRPC]
    public void RPC_DivideScore(int playerId)
    {
        if (playerScores.ContainsKey(playerId))
        {
            playerScores[playerId] = Mathf.FloorToInt(playerScores[playerId] / 2f);
        }
    }

    [PunRPC]
    public void RPC_EraseLastPiece()
    {
        FloorplanMapper.Instance.UndoLastPiece();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(GameStarted);
            stream.SendNext(currentTurn);
            stream.SendNext((int)currentCategory);
            stream.SendNext(categoryTimer);
            stream.SendNext(isReversed);

            stream.SendNext(playerScores.Count);
            foreach (var pair in playerScores)
            {
                stream.SendNext(pair.Key);
                stream.SendNext(pair.Value);
            }
        }
        else
        {
            GameStarted = (bool)stream.ReceiveNext();
            currentTurn = (int)stream.ReceiveNext();
            currentCategory = (Category)stream.ReceiveNext();
            categoryTimer = (float)stream.ReceiveNext();
            isReversed = (bool)stream.ReceiveNext();

            int count = (int)stream.ReceiveNext();
            playerScores.Clear();

            for (int i = 0; i < count; i++)
            {
                int key = (int)stream.ReceiveNext();
                float value = (float)stream.ReceiveNext();
                playerScores[key] = value;
            }
        }

        UpdateScoreUI();
    }
}