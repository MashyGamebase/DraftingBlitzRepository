using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CardHandEditorTool : EditorWindow
{
    private CardDatabase cardDatabase;
    private CardHand cardHand;

    private CardType selectedCardType;
    private List<Card> availableCards = new List<Card>();
    private int selectedCardIndex = 0;

    [MenuItem("Tools/Card Hand Editor")]
    public static void ShowWindow()
    {
        GetWindow<CardHandEditorTool>("Card Hand Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Card Hand Editor Tool", EditorStyles.boldLabel);

        cardDatabase = (CardDatabase)EditorGUILayout.ObjectField("Card Database", cardDatabase, typeof(CardDatabase), false);
        cardHand = (CardHand)EditorGUILayout.ObjectField("Card Hand", cardHand, typeof(CardHand), true);

        if (cardDatabase == null || cardHand == null)
        {
            EditorGUILayout.HelpBox("Assign both CardDatabase and CardHand to begin.", MessageType.Info);
            return;
        }

        selectedCardType = (CardType)EditorGUILayout.EnumPopup("Card Type", selectedCardType);

        if (GUILayout.Button("Load Cards"))
        {
            availableCards = cardDatabase.GetRandomCardsByType(selectedCardType, int.MaxValue, true);
            selectedCardIndex = 0;
        }

        if (availableCards.Count > 0)
        {
            string[] cardNames = availableCards.ConvertAll(card => card.CardName).ToArray();
            selectedCardIndex = EditorGUILayout.Popup("Select Card", selectedCardIndex, cardNames);

            if (GUILayout.Button("Add Card to Hand"))
            {
                Card selectedCard = availableCards[selectedCardIndex];
                if (selectedCard != null)
                {
                    Sprite icon = selectedCard.CardVisual;
                    cardHand.AddCard(icon, selectedCard);
                    Debug.Log($"Added '{selectedCard.CardName}' to hand.");
                }
            }
        }
    }
}
