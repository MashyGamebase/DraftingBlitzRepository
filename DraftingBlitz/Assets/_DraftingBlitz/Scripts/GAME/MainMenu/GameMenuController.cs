using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    public Image profileAvatar;
    public List<Sprite> profileAvatars;

    private void Start()
    {
        SetAvatar();
    }

    private void SetAvatar()
    {
        int index = PlayerPrefs.HasKey("ProfileAvatar") ? PlayerPrefs.GetInt("ProfileAvatar") : 0;
        profileAvatar.sprite = profileAvatars[index];
    }

    public void OnClick_VSAI()
    {
        FadeController.Instance.StartFakeLoading("GameScene", true);
    }
}