using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetAndSetNickname : MonoBehaviour
{
    FirebaseManager firebaseManager;
    public TMP_InputField nameInput;

    private void Start()
    {
        firebaseManager = FirebaseManager.Instance;

        GetName();
    }

    public void SetName()
    {
        PhotonNetwork.NickName = nameInput.text;

        if (firebaseManager != null)
        {
            firebaseManager.userNickname = nameInput.text;
            firebaseManager.SaveUserDataProxy();
        }
    }

    public void GetName()
    {
        if (firebaseManager != null)
        {
            nameInput.text = firebaseManager.userNickname;
        }
    }
}