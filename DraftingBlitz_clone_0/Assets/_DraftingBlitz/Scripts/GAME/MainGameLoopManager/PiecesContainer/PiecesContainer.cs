using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesContainer : MonoBehaviour
{
    public string pieceName;
    public bool filled = false;
    public int pieceID;
    public GameObject pieceImage;

    public void ActivatePiece(bool enabled)
    {
        filled = enabled;
        pieceImage.SetActive(enabled);
    }
}