using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIGameLoopManager : Singleton<AIGameLoopManager>
{
    public List<Player> players = new List<Player>();
    public int currentPlayerIndex = 0;
    public float turnDuration = 15f;
    private bool isGameRunning = true;

    public CurrentCategory currentCategory; // The active category for this turn

    public UnityEvent<int> OnTurnStart;
    public UnityEvent<int> OnTurnEnd;

    private void Start()
    {
        StartCoroutine(GameStartSequence());
    }

    IEnumerator GameStartSequence()
    {
        for (int i = 0; i < players.Count; i++)
        {
            bool isAI = (i != 0); // Assume player 0 is human, others are AI
            players[i].id = i;
            players[i].isAI = isAI;

            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (isGameRunning)
        {
            yield return StartCoroutine(PlayerTurn(players[currentPlayerIndex]));

            // Move to the next player
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

            // Change category each turn
            ChangeCategory();
        }
    }

    IEnumerator PlayerTurn(Player player)
    {
        Debug.Log($"Player {player.id}'s Turn - Category: {currentCategory}");

        OnTurnStart?.Invoke(player.id);

        player.isTurn = true;

        if (player.isAI)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f)); // Simulated AI "thinking time"
            player.AIMakeMove();
        }
        else
        {
            yield return StartCoroutine(HumanTurn(player));
        }

        yield return new WaitForSeconds(1f);

        player.isTurn = false;
        OnTurnEnd?.Invoke(player.id);
    }

    IEnumerator HumanTurn(Player player)
    {
        float timer = turnDuration;
        bool playedCard = false;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            if (player.HasValidMove())
            {
                // Wait for player input (handled elsewhere)
                yield return null;
            }
            else
            {
                break;
            }
        }

        if (!playedCard)
        {
            Debug.Log($"Player {player.id} passed!");
        }
    }

    /// <summary>
    /// Checks if a card type can be played in the current turn.
    /// </summary>
    public bool CanPlayCategory(CardType cardType)
    {
        switch (currentCategory)
        {
            // Special flag for the 2 special cards
            // Action Cards && Special Cards

            case CurrentCategory.Architectural:
                return cardType == CardType.Architectural;
            case CurrentCategory.Electrical:
                return cardType == CardType.Electrical;
            case CurrentCategory.Plumbing:
                return cardType == CardType.Plumbing;
            default:
                return false;
        }
    }

    /// <summary>
    /// Randomly changes the active category at the start of each round.
    /// </summary>
    private void ChangeCategory()
    {
        CurrentCategory[] categories = { CurrentCategory.Architectural, CurrentCategory.Electrical, CurrentCategory.Plumbing };
        currentCategory = categories[Random.Range(0, categories.Length)];
        Debug.Log($"New Category: {currentCategory}");
    }
}