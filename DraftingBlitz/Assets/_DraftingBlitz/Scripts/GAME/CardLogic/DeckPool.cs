using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DeckPool : Singleton<DeckPool>
{
    public GameObject cardPoolPrefab;

    public void AddToDeck(Vector3 destination)
    {
        GameObject obj = Instantiate(cardPoolPrefab, transform);
        obj.transform.position = transform.position;

        obj.transform.DOMove(destination, 0.70f)
        .OnComplete(() =>
        {
            Destroy(obj);
        });
    }
}