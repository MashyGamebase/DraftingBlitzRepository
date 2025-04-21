using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHand : MonoBehaviour
{
    public enum HandType { Bottom, Left, Right }
    public HandType handType;

    public GameObject cardPrefab;
    public float maxFanAngle = 30f; // Max angle spread for MyHand
    public float cardSpacing = 10f; // Distance between cards
    public float popHeight = 30f; // How much the card pops up
    public List<RectTransform> cards = new List<RectTransform>();
    public List<CardUIContainer> cardLogics = new List<CardUIContainer>();
    private RectTransform handRect;

    public bool ownHand = false;

    public int identifier;

    public bool cardRaised;

    public Transform poolOrigin;

    void Start()
    {
        handRect = GetComponent<RectTransform>();
    }

    public void AddCard(Sprite cardIcon, Card card)
    {
        GameObject newCard = Instantiate(cardPrefab, handRect);
        newCard.gameObject.GetComponent<CardUIContainer>().SetCardVisual(cardIcon, card, ownHand);

        cardLogics.Add(newCard.gameObject.GetComponent<CardUIContainer>());

        RectTransform cardRect = newCard.GetComponent<RectTransform>();

        Button button = newCard.GetComponent<Button>();
        if (button != null && handType == HandType.Bottom) // Only allow popping for bottom hand
        {
            int index = cards.Count;
            button.onClick.AddListener(() => PopCard(index));
        }

        cards.Add(cardRect);
        ArrangeCards();
    }

    public void RemoveCard(int cardChosen)
    {
        if (cards.Count == 0) return;

        // Remove the card's GameObject and return visual to the pool
        Destroy(cards[cardChosen].gameObject);
        CardPool.Instance.AddToPool(cardLogics[cardChosen].card.CardVisual, poolOrigin.position);

        // Remove from lists
        cards.RemoveAt(cardChosen);
        cardLogics.RemoveAt(cardChosen);

        // Reassign indices and listeners
        UpdateCardIndices();

        ArrangeCards();
    }

    /// <summary>
    /// Updates card indices and reassigns button listeners.
    /// </summary>
    private void UpdateCardIndices()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Button button = cards[i].GetComponent<Button>();
            if (button != null && handType == HandType.Bottom)
            {
                int index = i;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => PopCard(index));
            }
        }
    }

    private void ArrangeCards()
    {
        if (cards.Count == 0) return;

        int cardCount = cards.Count;

        if (handType == HandType.Bottom)
        {
            ArrangeBottomHand(cardCount);
        }
        else if (handType == HandType.Left)
        {
            ArrangeSideHand(cardCount, true); // Left
        }
        else if (handType == HandType.Right)
        {
            ArrangeSideHand(cardCount, false); // Right
        }
    }

    private void ArrangeBottomHand(int cardCount)
    {
        float angleStep = (cardCount > 1) ? maxFanAngle / (cardCount - 1) : 0f;
        float startAngle = -maxFanAngle / 2;

        for (int i = 0; i < cardCount; i++)
        {
            float angle = startAngle + i * angleStep;
            float xOffset = i * cardSpacing - (cardCount * cardSpacing) / 2f;

            cards[i].anchoredPosition = new Vector2(xOffset, 0);
            cards[i].rotation = Quaternion.Euler(0, 0, -angle);
        }
    }

    private void ArrangeSideHand(int cardCount, bool isLeft)
    {
        //float angleStep = 10f; // Small tilt for side hands
        float startY = -(cardCount * cardSpacing) / 2f;
        float rotationAngle = isLeft ? 90f : -90f; // Left = 90°, Right = -90°

        for (int i = 0; i < cardCount; i++)
        {
            float yOffset = startY + i * cardSpacing;

            cards[i].anchoredPosition = new Vector2(0, yOffset);
            cards[i].rotation = Quaternion.Euler(0, 0, rotationAngle);
        }
    }

    void PopCard(int index)
    {
        if (handType != HandType.Bottom) return; // Only pop from bottom hand

        if (cardLogics[index].isRaised)
        {
            TurnSystem.Instance.PlayCard(index);

            for (int i = 0; i < cards.Count; i++)
            {
                Vector2 pos = cards[i].anchoredPosition;
                pos.y = (i == index) ? popHeight : 0;
                cards[i].anchoredPosition = pos;
                cardLogics[i].isRaised = false;
            }

            return;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            Vector2 pos = cards[i].anchoredPosition;
            pos.y = (i == index) ? popHeight : 0;
            cards[i].anchoredPosition = pos;
            cardLogics[i].isRaised = false;
        }

        cardLogics[index].isRaised = true;
    }
}
