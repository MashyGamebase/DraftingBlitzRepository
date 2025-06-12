using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDisplayer : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    private void OnEnable()
    {
        rect.DOScaleX(1, 0.25f);
    }

    private void OnDisable()
    {
        rect.DOScaleX(0, 0.01f);
    }
}