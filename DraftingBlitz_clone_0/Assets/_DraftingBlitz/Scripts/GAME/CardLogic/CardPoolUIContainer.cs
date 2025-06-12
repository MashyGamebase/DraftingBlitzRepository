using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPoolUIContainer : MonoBehaviour
{
    public Image CardVisual;

    public void SetCardVisual(Sprite sprite)
    {
        CardVisual.sprite = sprite;
    }
}