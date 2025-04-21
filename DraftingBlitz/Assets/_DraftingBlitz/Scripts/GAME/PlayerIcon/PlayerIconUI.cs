using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIconUI : MonoBehaviour
{
    public Image avatarImage;
    public TMP_Text playerNameText;

    public void Setup(Sprite avatar, string playerName)
    {
        avatarImage.sprite = avatar;
        playerNameText.text = playerName;
    }
}