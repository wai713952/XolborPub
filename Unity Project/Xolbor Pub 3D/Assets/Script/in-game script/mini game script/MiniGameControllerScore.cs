using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameControllerScore : MonoBehaviour
{
    public List<string> miniGamePlayerNameArray = new List<string>();
    public List<float> miniGamePlayerScoreArray = new List<float>();
    public List<int> miniGmaePlayerClientIdArray = new List<int>();

    MiniGameControllerLobby miniGameControllerLobby;

    void Start()
    {
        miniGameControllerLobby = GetComponent<MiniGameControllerLobby>();

        string[] name = { "kim", "jessie", "ash", "huey" };
        float[] score = { 12f, 80f, 95f, 50f };
        int[] id = { 0, 1, 2, 3 };
        EvaluateMiniGameScore(name, score, id);
    }

    public void RecieveMiniGameScore(string playerName, float playerScore, int playerClientId)
    {
        // - recieving string of player's name and score from client
        // - called from client everytime their score has been created
        miniGamePlayerNameArray.Add(playerName);
        miniGamePlayerScoreArray.Add(playerScore);
        miniGmaePlayerClientIdArray.Add(playerClientId);

        if (miniGamePlayerNameArray.Count > miniGameControllerLobby.miniGamePlayerCurrent 
            && miniGamePlayerScoreArray.Count > miniGameControllerLobby.miniGamePlayerCurrent)
        {
            for (int i = 0; i < miniGameControllerLobby.miniGamePlayerCurrent; i++)
            {
                if (playerName == miniGamePlayerNameArray[i])
                {
                    miniGamePlayerNameArray.RemoveAt(i);
                    miniGamePlayerScoreArray.RemoveAt(i);
                    miniGmaePlayerClientIdArray.RemoveAt(i);
                    break;
                }
            }
        }

        //will execute when the game is over
        if (miniGameControllerLobby.isMiniGameOver)
        {

        }
    }
    public void EvaluateMiniGameScore(string[] playerName, float[] playerScore, int[] playerClientId)
    {
        // - sorting to find the best score index. return best score, score's index and player name with same index
        // - return string array that contains winner's name and score
        // - will execute from the server once the game is over

        //sorting to find best score
        float scoreBestValue = 0;
        int scoreBestIndex = -1;
        int winnerClientId = 99;

        for (int i = 0; i < playerName.Length; i++)
        {
            if (playerScore[i] >= scoreBestValue)
            {
                scoreBestValue = playerScore[i];
                scoreBestIndex = i;
                winnerClientId = playerClientId[i];
            }
        }

        string[] miniGamerWinnerNameAndScore = { playerName[scoreBestIndex], scoreBestValue.ToString() };
        print($"winner: {miniGamerWinnerNameAndScore[0]}___score: {miniGamerWinnerNameAndScore[1]}___id: {winnerClientId.ToString()}");
    }
}
