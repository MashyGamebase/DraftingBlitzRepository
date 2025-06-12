using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public CardDatabase cardDatabase;
    public int deckSize = 5;
    public int deckLimit = 7;
    public bool allowDuplicates = false;

    public GameObject cardPrefab;

    public Transform[] cardPrefabParents;

    [SerializeField] public List<Card> CachedDeck = new List<Card>();
    public List<CardHand> cardHands = new List<CardHand>();

    public GameObject handPrefab;

    public void ClearAllHands()
    {
        foreach (Transform child in cardPrefabParents)
        {
            Destroy(child.gameObject);
        }
        cardHands.Clear();
    }

    public void GenerateHand(int index, CardHand.HandType handType, bool ownHand)
    {
        cardHands[index].handType = handType;
        cardHands[index].ownHand = ownHand;

        CachedDeck.Clear();

        CachedDeck = cardDatabase.GenerateDeck(deckSize, allowDuplicates);
        foreach (Card card in CachedDeck)
        {
            cardHands[index].AddCard(card.CardVisual, card);
        }
    }

    public void DrawRandomCard(int index)
    {
        Card card = cardDatabase.GetRandomCard();

        cardHands[index].AddCard(card.CardVisual, card);
    }
}
