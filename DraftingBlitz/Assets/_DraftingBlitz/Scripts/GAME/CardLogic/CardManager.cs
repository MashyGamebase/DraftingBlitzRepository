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

    private void Start()
    {
        Deck = cardDatabase.GenerateDeck(deckSize, allowDuplicates);

        Debug.Log("Cards in Deck");

        foreach (Card card in Deck)
        {
            GameObject obj = Instantiate(cardPrefab, cardPrefabParent);
            obj.GetComponent<CardUIContainer>().SetCardVisual(card.CardVisual);

            Debug.Log(card.CardName);
        }
    }
}