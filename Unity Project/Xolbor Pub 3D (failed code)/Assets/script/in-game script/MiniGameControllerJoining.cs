using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MiniGameControllerJoining : MonoBehaviour
{
    //mini-game joining system
    //server

    public int miniGamePlayerCurrent;
    public int miniGamePlayerMinimum;
    public int miniGamePlayerMaximum;

    public bool isMiniGamePlayerFull;
    public bool isMiniGameStarted;
    public bool canPlayerJoinMiniGame;
    public string miniGameStatus;       //wating, playing, player full etc. will be display on top of the mini-game interaction point

    public string[] miniGamePlayerArray;
    public string[] miniGameScoreArray;


    public TMP_Text miniGameTextPrefab;

    public void Start()
    {
        int[] scores = { 1, 72 };
        string[] names = { "mark", "elon" };
        EvaluateMiniGame(names, scores);
        
    }

    public void JoinMiniGame(string playerName)
    {
        if (canPlayerJoinMiniGame == false)
        {
            //return and play decline sound
            return;
        }
        else if (canPlayerJoinMiniGame == true)
        {
            miniGamePlayerCurrent++;
        }
    }
    public void LeaveMiniGame()
    {
        miniGamePlayerCurrent--;
    }
    public bool StartMiniGame()     //StartMiniGame will be called from mini-game script,
                                    //which allow this script to work as lobby start function
                                    //for each game and also return bool to acknowledge the game has been started.                                    
    {
        isMiniGameStarted = true;
        return true;
    }
    public void StopMiniGame()      //StopMinigame will be called in this function
                                    //when the game is over by turn, or timeout,
                                    //or one of the players unfortunately disconnected,
                                    //in this condition will kick players off that game.
    {

    }
    public string[] EvaluateMiniGame(string[] playerNameArray, int[] playerScoreArray)
    {
        string[] miniGamerWinnerNameAndScore = { playerNameArray[0], playerScoreArray[1].ToString()};
        print(miniGamerWinnerNameAndScore[0] + " " + miniGamerWinnerNameAndScore[1]);
        return miniGamerWinnerNameAndScore;
    }

    private void FixedUpdate()
    {
        CheckMiniGameStatus();
    }
    private void CheckMiniGameStatus()
    {
        if (miniGamePlayerCurrent == 0 && isMiniGameStarted == false)
        {
            miniGameStatus = "Empty Lobby";
            canPlayerJoinMiniGame = false;
        }
        else if (miniGamePlayerCurrent < miniGamePlayerMaximum && isMiniGameStarted == false)
        {
            miniGameStatus = "Waiting For Player(s)";
            canPlayerJoinMiniGame = false;
        }
        else if (miniGamePlayerCurrent == miniGamePlayerMaximum && isMiniGameStarted == false)
        {
            miniGameStatus = "Players Full";
            canPlayerJoinMiniGame = false;
        }
        else if (miniGamePlayerCurrent < miniGamePlayerMaximum && isMiniGameStarted == true)
        {
            miniGameStatus = "Game Already Started";
            canPlayerJoinMiniGame = true;
        }
        else if (miniGamePlayerCurrent == miniGamePlayerMinimum && isMiniGameStarted == true)
        {
            miniGameStatus = "Game Already Started";
            canPlayerJoinMiniGame = true;
        }
    }
}
