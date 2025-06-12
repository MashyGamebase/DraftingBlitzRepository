using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Database", menuName = "Card Database")]
public class CardDatabase : ScriptableObject
{
    public List<Card> actionCards;
    public List<Card> architecturalCards;
    public List<Card> electricalCards;
    public List<Card> plumbingCards;
    public List<Card> specialCards;

    /// <summary>
    /// Get random cards from a specific type.
    /// </summary>
    public List<Card> GetRandomCardsByType(CardType type, int count, bool allowDuplicates = false)
    {
        switch (type)
        {
            case CardType.Action: return GetRandomCards(actionCards, count, allowDuplicates);
            case CardType.Architectural: return GetRandomCards(architecturalCards, count, allowDuplicates);
            case CardType.Electrical: return GetRandomCards(electricalCards, count, allowDuplicates);
            case CardType.Plumbing: return GetRandomCards(plumbingCards, count, allowDuplicates);
            case CardType.Special: return GetRandomCards(specialCards, count, allowDuplicates);
            default: return new List<Card>();
        }
    }

    /// <summary>
    /// Generate a deck using all card types with a specified size.
    /// </summary>
    public List<Card> GenerateDeck(int deckSize, bool allowDuplicates = false)
    {
        List<Card> deck = new List<Card>();

        List<List<Card>> allCardLists = new List<List<Card>> { actionCards, architecturalCards, electricalCards, plumbingCards, specialCards };
        List<Card> allCards = new List<Card>();

        // Collect all cards from all categories
        foreach (var cardList in allCardLists)
        {
            allCards.AddRange(cardList.FindAll(card => card != null && !card.dealt));
        }

        if (allCards.Count == 0) return deck; // Return empty if no undealt cards exist

        if (allowDuplicates)
        {
            for (int i = 0; i < deckSize; i++)
            {
                Card card = allCards[Random.Range(0, allCards.Count)];
                deck.Add(card);
                card.dealt = true;
            }
        }
        else
        {
            List<Card> tempList = new List<Card>(allCards);
            for (int i = 0; i < Mathf.Min(deckSize, tempList.Count); i++)
            {
                int randomIndex = Random.Range(0, tempList.Count);
                Card selectedCard = tempList[randomIndex];
                deck.Add(selectedCard);
                selectedCard.dealt = true;
                tempList.RemoveAt(randomIndex);
            }
        }

        return deck;
    }

    /// <summary>
    /// Returns a single random card from all categories.
    /// </summary>
    public Card GetRandomCard()
    {
        List<Card> availableCards = new List<Card>();

        if (actionCards != null && actionCards.Count > 0)
            availableCards.AddRange(actionCards);
        if (architecturalCards != null && architecturalCards.Count > 0)
            availableCards.AddRange(architecturalCards);
        if (electricalCards != null && electricalCards.Count > 0)
            availableCards.AddRange(electricalCards);
        if (plumbingCards != null && plumbingCards.Count > 0)
            availableCards.AddRange(plumbingCards);
        if (specialCards != null && specialCards.Count > 0)
            availableCards.AddRange(specialCards);

        if (availableCards.Count == 0)
        {
            Debug.LogWarning("CardDatabase: No available cards to draw.");
            return null;
        }

        return availableCards[Random.Range(0, availableCards.Count)];
    }

    /// <summary>
    /// Generic function to get random cards from a given list.
    /// </summary>
    private List<Card> GetRandomCards(List<Card> cardList, int count, bool allowDuplicates)
    {
        List<Card> result = new List<Card>();
        if (cardList == null || cardList.Count == 0) return result;

        List<Card> undealtCards = cardList.FindAll(card => card != null && !card.dealt);

        if (undealtCards.Count == 0) return result;

        if (allowDuplicates)
        {
            for (int i = 0; i < count; i++)
            {
                Card card = undealtCards[Random.Range(0, undealtCards.Count)];
                result.Add(card);
                card.dealt = true;
            }
        }
        else
        {
            List<Card> tempList = new List<Card>(undealtCards);
            for (int i = 0; i < Mathf.Min(count, tempList.Count); i++)
            {
                int randomIndex = Random.Range(0, tempList.Count);
                Card selectedCard = tempList[randomIndex];
                result.Add(selectedCard);
                selectedCard.dealt = true;
                tempList.RemoveAt(randomIndex);
            }
        }

        return result;
    }

    /// <summary>
    /// Finds a card by its name across all card categories.
    /// </summary>
    public Card GetCardByName(string cardName)
    {
        List<List<Card>> allCardLists = new List<List<Card>>
        {
            actionCards,
            architecturalCards,
            electricalCards,
            plumbingCards,
            specialCards
        };

        foreach (var cardList in allCardLists)
        {
            foreach (var card in cardList)
            {
                if (card != null && card.CardName == cardName)
                    return card;
            }
        }

        Debug.LogWarning($"Card with name '{cardName}' not found in the database.");
        return null;
    }

    /// <summary>
    /// Resets the status of cards in the database
    /// </summary>
    public void ResetCardDatabase()
    {
        List<List<Card>> allCardLists = new List<List<Card>>
        {
            actionCards,
            architecturalCards,
            electricalCards,
            plumbingCards,
            specialCards
        };

        foreach (var cardList in allCardLists)
        {
            foreach (var card in cardList)
            {
                if (card != null)
                    card.dealt = false;
            }
        }
    }
}
