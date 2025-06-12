using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplayer : MonoBehaviour
{
    public CardInfoContainer CardInfo;
    public GameObject CardInfoBlocker;
    public Sprite ImageToDisplay;

    public Button OnDisplayButton;

    private void Start()
    {
        OnDisplayButton.onClick.AddListener(DisplayCard);
    }

    void DisplayCard()
    {
        CardInfoBlocker.SetActive(true);
        CardInfo.SetCardDisplay(ImageToDisplay);
    }
}