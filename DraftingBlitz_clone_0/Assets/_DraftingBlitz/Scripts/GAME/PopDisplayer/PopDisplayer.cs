using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopDisplayer : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    private void OnEnable()
    {
        rect.DOScaleX(1, 0.75f);
        rect.DOScaleY(1, 0.75f);
    }

    private void OnDisable()
    {
        rect.DOScaleX(0, 0.01f);
        rect.DOScaleY(0, 0.01f);
    }
}