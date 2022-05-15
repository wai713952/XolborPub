using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(MiniGameControllerScore))]
[RequireComponent(typeof(ObjectInteractable))]

public class MiniGameControllerLobby : MonoBehaviour
{
    //mini-game joining system
    //server

    public int miniGamePlayerCurrent;
    public int miniGamePlayerMinimum;
    public int miniGamePlayerMaximum;
    public int miniGamePlayerVoteCount;

    public int miniGameTimeMax;
    public int miniGameTimeCurrent;

    public bool isMiniGameStarted;
    public bool isMiniGameOver;
    public bool canPlayerJoinMiniGame;
    public string miniGameStatus;       //wating, playing, player full etc. will be display on top of the mini-game interaction point

    public TMP_Text miniGameTextPrefabState;
    public TMP_Text miniGmaeTextPrefab;

    private ObjectInteractable objectInteractable;
    public List<bool> voteList = new List<bool>();

    public void Start()
    {
        objectInteractable = GetComponent<ObjectInteractable>();
        InvokeRepeating("DisplayPlayerInMiniGame", 1f, 1f);
        InvokeRepeating("MiniGameVoteCheck", 1f, 1f);
    }

    public bool StartMiniGame()                                   
    {
        // - StartMiniGame will be called from mini-game script
        // - allow this script to work as lobby start function for each game
        // - return bool to acknowledge the game is started

        isMiniGameStarted = true;
        return true;
    }
    public void StopMiniGame()      
    {
        // - StopMinigame will be called in this function when the game is over by turn, or timeout,
        // - or one of the players unfortunately disconnected, in this condition will kick players off that game.
    }

    private void FixedUpdate()
    {
        CheckMiniGameStatus();
    }
    private void CheckMiniGameStatus()
    {
        miniGamePlayerCurrent = objectInteractable.interactorNameList.Count;

        if (miniGamePlayerCurrent == 0 && isMiniGameStarted == false)
        {
            miniGameStatus = "Empty Lobby";
            canPlayerJoinMiniGame = true;
        }
        else if (miniGamePlayerCurrent < miniGamePlayerMaximum && isMiniGameStarted == false)
        {
            miniGameStatus = "Waiting For Player(s)";
            canPlayerJoinMiniGame = true;
        }
        else if (miniGamePlayerCurrent == miniGamePlayerMaximum && isMiniGameStarted == false)
        {
            miniGameStatus = "Players Full";
            canPlayerJoinMiniGame = false;
        }
        else if (miniGamePlayerCurrent <= miniGamePlayerMaximum && isMiniGameStarted == true)
        {
            miniGameStatus = "Game Already Started";
            canPlayerJoinMiniGame = false;
        }
    }

    private void DisplayPlayerInMiniGame()
    {
        string tempPlayerInLobby = "";
        foreach (string name in objectInteractable.interactorNameList)
        {
            tempPlayerInLobby = tempPlayerInLobby + (" ") + name;
        }
    }
    public void MiniGameVoteStart(bool vote)
    {
        if (vote == true)
        {
            voteList.Add(true);
        }
        else
        {
            voteList.Remove(true);
        }
    }
    private void MiniGameVoteCheck()
    {
        if(voteList.Count == 0) { return; }

        for (int i = 0; i < miniGamePlayerCurrent; i++)
        {
            if (voteList[i] == true)
            {
                miniGamePlayerVoteCount++;
            }
            if (miniGamePlayerVoteCount == miniGamePlayerCurrent 
                && miniGamePlayerCurrent >= miniGamePlayerMinimum)
            {
                MiniGameStarted();
                CancelInvoke("MiniGameVoteCheck");
                return;
            }
        }
        miniGamePlayerVoteCount = 0;
    }
    private void MiniGameStarted()
    {
        print("game started");
        miniGameTimeCurrent = miniGameTimeMax;
        isMiniGameStarted = true;
        InvokeRepeating("MiniGameTimeCount", 0f, 1f);
    }
    private void MiniGameTimeCount()
    {
        //recieve the max time
        //decrease time by 1 every second
        if (isMiniGameStarted == false) { return; }

        miniGameTimeCurrent -= 1;
        if (Mathf.CeilToInt(miniGameTimeCurrent) <= 0)
        {
            MiniGameOver();
            CancelInvoke("MiniGameTimeCount");
            Invoke("MiniGameReset", 3f);
        }
    }
    private void MiniGameOver()
    {
        // - wait for a moment before reset the minigame
        // - make the players leave the minigame

        isMiniGameOver = true;
        print("game over");
    }
    private void MiniGameReset()
    {
        isMiniGameStarted = false;
        isMiniGameOver = false;
        canPlayerJoinMiniGame = true;
        
        objectInteractable.InteractionCDismissAll();
        objectInteractable.pressCount = 0;

        voteList.Clear();
        miniGamePlayerVoteCount = 0;

        InvokeRepeating("MiniGameVoteCheck", 1f, 1f);
        print("minigame reset");
    }
}
