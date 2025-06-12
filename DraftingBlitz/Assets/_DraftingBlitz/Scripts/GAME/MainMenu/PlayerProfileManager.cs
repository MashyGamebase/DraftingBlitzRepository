using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using TMPro;

public class PlayerProfileManager : MonoBehaviour
{
    public TMP_InputField PlayerName;
    public Image PlayerFaceAvatar;
    public Image PlayerBodyAvatar;

    public List<Sprite> faceAvatars;
    public List<Sprite> bodyAvatars;


    private void Start()
    {
        SetupPlayerProfile(PhotonNetwork.NickName, PlayerPrefs.HasKey("ProfileAvatar") ? PlayerPrefs.GetInt("ProfileAvatar") : 0);
    }

    void SetupPlayerProfile(string name, int avatarIndex)
    {
        PlayerName.text = name;
        PlayerFaceAvatar.sprite = faceAvatars[avatarIndex];
        PlayerBodyAvatar.sprite = bodyAvatars[avatarIndex];
    }
}
