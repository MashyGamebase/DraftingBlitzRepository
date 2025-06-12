using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIconSetter : MonoBehaviour
{
    public List<PlayerIconUI> playerIconUIs = new List<PlayerIconUI>();

    public List<Sprite> playerIcons = new List<Sprite>();

    public void SetNameUI(int index, string name, int iconIndex)
    {
        playerIconUIs[index].Setup(playerIcons[iconIndex], name);
    }
}