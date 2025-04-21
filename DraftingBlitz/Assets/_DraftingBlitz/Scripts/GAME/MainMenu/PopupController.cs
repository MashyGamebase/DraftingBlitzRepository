using DG.Tweening;
using TMPro;
using UnityEngine;

public class PopupController : Singleton<PopupController>
{
    public RectTransform popupGameObject;
    public TMP_Text popupText;

    private Vector2 startPosition; // Store the initial position

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        startPosition = popupGameObject.anchoredPosition; // Store initial position
    }

    public void PopupNotif(string text, float popupTime)
    {
        popupText.text = text;

        // Create a sequence to animate the popup
        Sequence popupSequence = DOTween.Sequence();
        popupSequence.Append(popupGameObject.DOAnchorPosY(-25, 1)) // Move down
                     .AppendInterval(popupTime) // Wait
                     .Append(popupGameObject.DOAnchorPosY(startPosition.y, 1)); // Move back up
    }
}
