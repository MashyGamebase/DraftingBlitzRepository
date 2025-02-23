using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUIContainer : MonoBehaviour
{
    public Image CardVisual;
    public Card card;
    public bool isRaised;

    public void SetCardVisual(Sprite sprite, Card getCard, bool ifOwnHand)
    {
        if(ifOwnHand)
            CardVisual.sprite = sprite;

        card = getCard;
    }

    public CardType GetCardType()
    {
        return card.CardType;
    }
}