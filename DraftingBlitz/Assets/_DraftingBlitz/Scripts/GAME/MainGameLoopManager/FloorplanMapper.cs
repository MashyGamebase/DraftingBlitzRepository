using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorplanMapper : MonoBehaviourPunCallbacks, IPunObservable
{
    public static FloorplanMapper Instance;

    private int filledPieces = 0; // how many pieces are filled
    private bool gameEnded = false; // prevent multiple calls

    [SerializeField] EndGameEvaluator endGameEval;
    [SerializeField] TurnSystem turnSystem;

    private string lastAppliedCardName;
    private string lastAppliedCategory;
    public float lastAppliedPoints;


    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        for(int i = 0; i < pieces.Count; i++)
        {
            pieces[i].pieceID = i;
        }
    }

    [Header("Symbol Pieces")]
    public List<PiecesContainer> pieces = new List<PiecesContainer>();

    [Header("Category Headers")]
    public CanvasGroup architecturalHeader;
    public CanvasGroup electricalHeader;
    public CanvasGroup plumbingHeader;

    public void ApplyPiece(string cardName, string category)
    {
        lastAppliedCardName = cardName;
        lastAppliedCategory = category;

        List<PiecesContainer> matchingPieces = pieces.FindAll(p => p.pieceName.Contains(cardName));

        foreach (var piece in matchingPieces)
        {
            Debug.Log(piece.pieceName);
        }

        if (matchingPieces.Count == 0)
        {
            Debug.LogWarning($"No piece found matching card name: {cardName}");
            return;
        }

        // Try to find the first unfilled piece
        PiecesContainer targetPiece = matchingPieces.Find(p => !p.filled);

        if(targetPiece != null)
            photonView.RPC("RPC_PutInPiece", RpcTarget.AllBuffered, targetPiece.pieceID, category);
    }

    public void UndoLastPiece()
    {
        if (!string.IsNullOrEmpty(lastAppliedCardName))
        {
            // Logic to visually undo/remove the last applied card
            // and reset last applied
            RemovePiece(lastAppliedCardName);
            lastAppliedCardName = null;
            lastAppliedCategory = null;
        }
    }

    public void RemovePiece(string cardName)
    {
        // Find all pieces matching the card name
        List<PiecesContainer> matchingPieces = pieces.FindAll(p => p.pieceName.Contains(cardName) && p.filled);

        if (matchingPieces == null || matchingPieces.Count == 0)
        {
            Debug.LogWarning($"No filled piece found with name: {cardName}");
            return;
        }

        // We assume the most recent match is the last one filled
        PiecesContainer targetPiece = matchingPieces[matchingPieces.Count - 1];

        if (targetPiece != null)
        {
            photonView.RPC("RPC_DeletePiece", RpcTarget.AllBuffered, targetPiece.pieceID);
            Debug.Log($"Removed piece with ID: {targetPiece.pieceID} for card: {cardName}");
        }
    }

    public void RequestDeletePiece(int playerId)
    {
        PiecesContainer toDelete = pieces.FindLast(p => p.filled);

        if (toDelete != null)
        {
            photonView.RPC("RPC_DeletePiece", RpcTarget.AllBuffered, toDelete.pieceID);
        }
    }

    [PunRPC]
    public void RPC_PutInPiece(int pieceID, string category)
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].pieceID == pieceID)
            {
                if (!pieces[i].filled) // Only count once
                {
                    pieces[i].ActivatePiece(true);
                    filledPieces++;

                    // Check if game finished
                    if (filledPieces >= pieces.Count && !gameEnded)
                    {
                        gameEnded = true;
                        StartCoroutine(HandleGameEnd());
                    }
                }
                else
                {
                    // Already filled
                    pieces[i].ActivatePiece(true);
                }

                SetCategoryOpacity(category);
                break;
            }
        }
    }

    [PunRPC]
    public void RPC_DeletePiece(int pieceID)
    {
        try
        {
            PiecesContainer pieceToDelete = pieces.Find(p => p.pieceID == pieceID);

            if (pieceToDelete == null)
            {
                Debug.LogWarning($"No piece found with ID: {pieceID}");
                return;
            }

            if (!pieceToDelete.filled)
            {
                Debug.LogWarning($"Piece {pieceID} is already empty. Cannot delete.");
                return;
            }

            pieceToDelete.ActivatePiece(false); // Deactivate/hide
            filledPieces = Mathf.Max(0, filledPieces - 1);
            Debug.Log($"Deleted piece {pieceID}. Remaining filled pieces: {filledPieces}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error deleting piece {pieceID}: {ex.Message}");
        }
    }

    void SetCategoryOpacity(string category)
    {
        switch(category)
        {
            case "Architectural":
                architecturalHeader.alpha = 1;
                electricalHeader.alpha = 0.25f;
                plumbingHeader.alpha = 0.25f;
                break;
            case "Electrical":
                architecturalHeader.alpha = 0.25f;
                electricalHeader.alpha = 1;
                plumbingHeader.alpha = 0.25f;
                break;
            case "Plumbing":
                architecturalHeader.alpha = 0.25f;
                electricalHeader.alpha = 0.25f;
                plumbingHeader.alpha = 1;
                break;
        }
    }

    private IEnumerator HandleGameEnd()
    {
        // TODO: Show your ranking screen here
        Debug.Log("All pieces filled! Showing ranking...");

        yield return new WaitForSeconds(.25f); // Wait for players to see it (optional)

        // Name, TotalScore, TotalPoitns, TotalCoins, TotalBlitzCardPlayed, TotalActionCardsPlayed, TotalSpecialCardsPlayed, OverallScore, playerScoresDict
        endGameEval.ShowEndGameScreen(turnSystem.localPlayerId,
                                      PhotonNetwork.NickName, 
                                      turnSystem.playerScores[turnSystem.localPlayerId],
                                      10,
                                      turnSystem.totalCardsPlayed,
                                      0,
                                      0,
                                      (turnSystem.playerScores[turnSystem.localPlayerId] + turnSystem.totalCardsPlayed),
                                      turnSystem.playerScores);
    }

#if UNITY_EDITOR
    [ContextMenu("END GAME")]
#endif
    [PunRPC]
    public void RPC_DEBUG_EndGame()
    {
        gameEnded = true;
        StartCoroutine(HandleGameEnd());
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(filledPieces);
            stream.SendNext(gameEnded);
        }
        else
        {
            filledPieces = (int)stream.ReceiveNext();
            gameEnded = (bool)stream.ReceiveNext();
        }
    }
}