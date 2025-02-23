using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public CardDatabase cardDatabase;
    public int deckSize;
    public bool allowDuplicates = false;

    public GameObject cardPrefab;
    public Transform cardPrefabParent;

    [SerializeField] private List<Card> Deck = new List<Card>();

    public List<CardHand> cardHand;

    private void Start()
    {
        Deck = cardDatabase.GenerateDeck(deckSize, allowDuplicates);

        Debug.Log("Cards in Deck");

        for (int i = 0; i < cardHand.Count; i++)
        {
            foreach(Card card in Deck)
            {
                cardHand[i].AddCard(card.CardVisual, card);

                Debug.Log(card.CardName);
            }
        }
    }
}