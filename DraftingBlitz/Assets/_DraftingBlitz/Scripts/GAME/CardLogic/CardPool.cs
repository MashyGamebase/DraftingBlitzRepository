using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardPool : Singleton<CardPool>
{
    public float minRotation = -35f;
    public float maxRotation = 35f;

    public GameObject cardPoolPrefab;
    public Transform cardPoolParent;

    public float smoothSpeed = 5f;

    public void AddToPool(Sprite cardVisual, Vector3 position)
    {
        GameObject obj = Instantiate(cardPoolPrefab, cardPoolParent);
        obj.transform.position = position;
        obj.GetComponent<CardPoolUIContainer>().SetCardVisual(cardVisual);

        obj.transform.DOMove(transform.position, 0.70f)
        .OnComplete(() =>
        {
            obj.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(minRotation, maxRotation)));
        });
    }
}