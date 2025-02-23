using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionHandler : MonoBehaviour
{
    public List<Sprite> FullBody_avatars;
    public List<Sprite> Face_avatars;

    public Button nextButton;
    public Button previousButton;
    public Button selectAvatarButton;
    public Button confirmSelectionButton;

    public Image FBAvatar;
    public Image FaceAvatar;

    public TMP_InputField nameInputField;

    [SerializeField] private int currentSelectedIndex;

    private void Start()
    {
        currentSelectedIndex = 0;

        nextButton.onClick.AddListener(NextAvatar);
        previousButton.onClick.AddListener(PreviousAvatar);
        selectAvatarButton.onClick.AddListener(SelectAvatar);

        confirmSelectionButton.onClick.AddListener(OnClick_Confirm);
    }

    private void NextAvatar()
    {
        currentSelectedIndex = (currentSelectedIndex + 1) % FullBody_avatars.Count;
        FBAvatar.sprite = FullBody_avatars[currentSelectedIndex];
    }

    private void PreviousAvatar()
    {
        currentSelectedIndex = (currentSelectedIndex - 1 + FullBody_avatars.Count) % FullBody_avatars.Count;
        FBAvatar.sprite = FullBody_avatars[currentSelectedIndex];
    }

    private void SelectAvatar()
    {
        FaceAvatar.sprite = Face_avatars[currentSelectedIndex];
        PlayerPrefs.SetInt("ProfileAvatar", currentSelectedIndex);
        PlayerPrefs.Save();
    }

    public void OnClick_Confirm()
    {
        FadeController.Instance.StartFakeLoading("GameMenu");
    }
}