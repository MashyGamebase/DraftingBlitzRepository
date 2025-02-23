using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoopManager : MonoBehaviour
{
    public List<Player> players = new List<Player>();
    const int PLAYER_COUNT = 3; // Unless the player count changes its constantly at 3

    /* 1. Game Start sequence
     * 2. Initializes all players in scene
     * 3. Gives each player a hand (Deck of 5)
     * 4. Shuffle animation
     * 5. Game Loop Start
     */

    private void Start()
    {
        
    }

    IEnumerator GameStartSequence()
    {
        for (int i = 0; i < PLAYER_COUNT; i++)
        {
            //Player new_player = new Player(0, i);
            //players.Add(new_player);

            yield return new WaitForSeconds(0.2f);
        }
    }
}



public enum CurrentCategory
{
    Architectural,
    Electrical,
    Plumbing,
}