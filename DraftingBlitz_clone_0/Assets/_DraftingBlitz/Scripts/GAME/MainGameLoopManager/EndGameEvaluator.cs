using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class EndGameEvaluator : MonoBehaviour
{
    public GameObject EndGameBlocker;

    public ScoreboardUiContainer FirstPlaceBlocker, SecondPlaceBlocker, ThirdplaceBlocker;

    public Button goBackToMainMenuBtn;

    private void Start()
    {
        goBackToMainMenuBtn.onClick.AddListener(GoBackToMainMenu);
    }

    public void ShowEndGameScreen(int playerID, string nickName, float _totalPoints, int _totalCoins, int totalCardsPlayed, int totalActionCards, int totalSpecialCards, float overallScore, Dictionary<int, float> _playerScores)
    {
        var sorted = _playerScores.OrderByDescending(pair => pair.Value).ToList();

        int position = sorted.FindIndex(pair => pair.Key == playerID) + 1;

        EndGameBlocker.SetActive(true);

        switch (position)
        {
            case 1:
                FirstPlaceBlocker.gameObject.SetActive(true);
                FirstPlaceBlocker.ApplyText(nickName,
                                            _totalPoints.ToString(),
                                            _totalCoins.ToString(),
                                            totalCardsPlayed.ToString(),
                                            totalActionCards.ToString(),
                                            totalCardsPlayed.ToString(),
                                            overallScore.ToString());
                break;
            case 2:
                SecondPlaceBlocker.gameObject.SetActive(true);
                SecondPlaceBlocker.ApplyText(nickName,
                                            _totalPoints.ToString(),
                                            _totalCoins.ToString(),
                                            totalCardsPlayed.ToString(),
                                            totalActionCards.ToString(),
                                            totalCardsPlayed.ToString(),
                                            overallScore.ToString());
                break;
            case 3:
                ThirdplaceBlocker.gameObject.SetActive(true);
                ThirdplaceBlocker.ApplyText(nickName,
                                            _totalPoints.ToString(),
                                            _totalCoins.ToString(),
                                            totalCardsPlayed.ToString(),
                                            totalActionCards.ToString(),
                                            totalCardsPlayed.ToString(),
                                            overallScore.ToString());
                break;
        }
    }

    public void GoBackToMainMenu()
    {
        PhotonNetwork.LeaveRoom();
        FadeController.Instance.StartFakeLoading("GameMenu");
    }
}
