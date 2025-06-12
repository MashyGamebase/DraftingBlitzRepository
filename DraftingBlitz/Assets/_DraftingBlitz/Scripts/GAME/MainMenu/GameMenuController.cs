using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviourPunCallbacks
{
    public Image profileAvatar;
    public List<Sprite> profileAvatars;
    public Button MenuButton;

    public GameObject SettingsTab;

    [SerializeField]
    bool isSettingsOpen = false;

    public Button VSMatchButton;
    public TMP_InputField roomID;


    public PhotonCreateJoinRoom roomMaker;

    private void Start()
    {
        SetAvatar();
        AssignButtons();
    }

    void AssignButtons()
    {
        MenuButton.onClick.AddListener(ToggleMainMenu);
        VSMatchButton.onClick.AddListener(CreateRoom);
    }

    public void ToggleMainMenu()
    {
        if (isSettingsOpen)
        {
            isSettingsOpen = false;
            SettingsTab.SetActive(false);
        }
        else if (!isSettingsOpen)
        {
            isSettingsOpen = true;
            SettingsTab.SetActive(true);
        }
    }

    public void CreateRoom()
    {
        roomMaker.TryJoinOrCreateRoom(roomID.text);
    }

    public void LogoutUserAndLoadMainMenu()
    {
        if(FirebaseManager.Instance != null)
        {
            if (FirebaseManager.Instance.user.IsValid())
                FirebaseManager.Instance?.LogoutUser();
        }

        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        FadeController.Instance.StartFakeLoading("MainMenu", true);
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