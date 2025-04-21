using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    public Image profileAvatar;
    public List<Sprite> profileAvatars;
    public Button MenuButton;


    private void Start()
    {
        SetAvatar();
        AssignButtons();
    }

    void AssignButtons()
    {
        MenuButton.onClick.AddListener(LogoutUserAndLoadMainMenu);
    }

    void LogoutUserAndLoadMainMenu()
    {
        FirebaseManager.Instance?.LogoutUser();
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