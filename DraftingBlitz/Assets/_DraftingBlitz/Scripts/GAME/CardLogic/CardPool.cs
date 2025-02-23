using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPool : Singleton<CardPool>
{
    public float minRotation = -35f;
    public float maxRotation = 35f;

    public GameObject cardPoolPrefab;
    public Transform cardPoolParent;

    public void AddToPool(Sprite cardVisual)
    {
        GameObject obj = Instantiate(cardPoolPrefab, cardPoolParent);
        obj.GetComponent<CardPoolUIContainer>().SetCardVisual(cardVisual);
        obj.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(minRotation, maxRotation)));
    }
}