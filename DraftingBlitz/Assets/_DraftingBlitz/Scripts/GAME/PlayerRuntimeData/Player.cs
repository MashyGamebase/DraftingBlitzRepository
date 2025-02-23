using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public bool isAI;
    public CardHand cardHand;

    public bool isTurn;

    public Player(int id, bool isAI)
    {
        this.id = id;
        this.isAI = isAI;
    }

    public bool HasValidMove()
    {
        foreach (RectTransform card in cardHand.cards)
        {
            CardUIContainer container = card.GetComponent<CardUIContainer>();
            if (container != null)
            {
                CardType type = container.GetCardType();
                if (AIGameLoopManager.Instance.CanPlayCategory(type))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void AIMakeMove()
    {
        if (HasValidMove())
        {
            PlayCard();
        }
        else
        {
            Debug.Log($"AI Player {id} passed!");
        }
    }

    public void PlayCard()
    {
        if (!isTurn)
            return;

        for (int i = 0; i < cardHand.cards.Count; i++)
        {
            CardUIContainer container = cardHand.cards[i].GetComponent<CardUIContainer>();
            if (container != null)
            {
                CardType type = container.GetCardType();
                if (AIGameLoopManager.Instance.CanPlayCategory(type))
                {
                    Debug.Log($"AI Player {id} played a {type} card!");
                    cardHand.RemoveCard(i);
                    return;
                }
            }
        }
    }
}