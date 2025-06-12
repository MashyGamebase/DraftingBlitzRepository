using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public string CardName;
    public Sprite CardVisual;
    public CardType CardType;
    public float Points;

    public bool dealt = false;
}