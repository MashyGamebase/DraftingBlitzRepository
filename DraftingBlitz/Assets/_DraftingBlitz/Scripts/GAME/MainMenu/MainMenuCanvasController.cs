using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMenuCanvasController : Singleton<MainMenuCanvasController>
{
    public CanvasGroup loginRegisterPanel;
    public CanvasGroup bottomPartPanel;

    public void ShowLoginRegisterPanel()
    {
        bottomPartPanel.GetComponent<RectTransform>().DOAnchorPosY(-120f, 0.8f)
        .OnComplete(() =>
        {
            loginRegisterPanel.GetComponent<RectTransform>().DOScale(1, 0.4f);
        });
    }

    public void HideLoginRegisterPanel()
    {
        loginRegisterPanel.GetComponent<RectTransform>().DOScale(0, 0.4f)
        .OnComplete(() =>
        {
            bottomPartPanel.GetComponent<RectTransform>().DOAnchorPosY(120f, 0.8f);
        });
    }
}