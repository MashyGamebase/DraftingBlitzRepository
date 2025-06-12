using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardUiContainer : MonoBehaviour
{
    public TMP_Text PlayerName;
    public TMP_Text TotalPoints;
    public TMP_Text TotalCoins;
    public TMP_Text TotalBlitzCardsPlayed;
    public TMP_Text TotalActionCardsPlayed;
    public TMP_Text TotalSpecialCardsPlayed;
    public TMP_Text OverallScore;

    public void ApplyText(string _playerName, string _totalPoints, string _totalCoins, string _totalBlitzCardsPlayed, string _totalActionCardsPlayed, string _totalSpecialCardsPlayed, string _overallScore)
    {
        PlayerName.text = _playerName;
        TotalPoints.text = _totalPoints;
        TotalCoins.text = _totalCoins;
        TotalBlitzCardsPlayed.text = _totalBlitzCardsPlayed;
        TotalActionCardsPlayed.text = _totalActionCardsPlayed;
        TotalSpecialCardsPlayed.text = _totalSpecialCardsPlayed;
        OverallScore.text = _overallScore;
    }
}