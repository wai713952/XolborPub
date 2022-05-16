using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

[RequireComponent(typeof(MiniGameControllerScore))]
[RequireComponent(typeof(ObjectInteractable))]

public class MiniGameControllerLobby : NetworkBehaviour
{
    //mini-game joining system
    //server operated script

    //public int miniGamePlayerCurrent;

    public NetworkVariable<int> miniGamePlayerCurrentNetwork = new NetworkVariable<int>();
    public int miniGamePlayerMinimum;
    public int miniGamePlayerMaximum;

    public bool isMiniGameStarted;
    public bool isMiniGameOver;
    public bool canPlayerJoinMiniGame;
    public bool isLaunchingStart = false;
    public string miniGameStatus;       //wating, playing, player full etc. will be display on top of the mini-game interaction point

    public TMP_Text miniGameTextPrefabState;
    public TMP_Text miniGmaeTextPrefab;

    private ObjectInteractable objectInteractable;
    private MiniGameControllerScore miniGameControllerScore;

    public List<bool> voteList = new List<bool>();
    public NetworkVariable<int> voteCountNetwork = new NetworkVariable<int>();
    public NetworkVariable<NetworkString> miniGameStatusNetwork = new NetworkVariable<NetworkString>();


    private void Start()
    {
        objectInteractable = GetComponent<ObjectInteractable>();
        miniGameControllerScore = GetComponent<MiniGameControllerScore>();
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            CheckMiniGameStatus();
            CheckMiniGamesVote();
        }
    }
    private void CheckMiniGameStatus()
    {
        miniGamePlayerCurrentNetwork.Value = objectInteractable.interactorNameList.Count;
        miniGameStatusNetwork.Value = miniGameStatus;


        if (miniGamePlayerCurrentNetwork.Value == 0 && isMiniGameStarted == false)
        {
            miniGameStatus = "Empty Lobby";
            canPlayerJoinMiniGame = true;
        }
        else if (miniGamePlayerCurrentNetwork.Value < miniGamePlayerMaximum && isMiniGameStarted == false)
        {
            miniGameStatus = "Waiting For Player(s)";
            canPlayerJoinMiniGame = true;
        }
        else if (miniGamePlayerCurrentNetwork.Value == miniGamePlayerMaximum && isMiniGameStarted == false)
        {
            miniGameStatus = "Players Full";
            canPlayerJoinMiniGame = false;
        }
        else if (miniGamePlayerCurrentNetwork.Value <= miniGamePlayerMaximum && miniGamePlayerCurrentNetwork.Value >= miniGamePlayerMinimum &&
            isMiniGameStarted == true)
        {
            miniGameStatus = "Game Already Started";
            canPlayerJoinMiniGame = false;
        }
    }
    private void CheckMiniGamesVote()
    {
        voteCountNetwork.Value = voteList.Count;
        if (voteCountNetwork.Value == 0 || isMiniGameStarted == true) { return; }


        if (voteCountNetwork.Value == miniGamePlayerCurrentNetwork.Value
            && miniGamePlayerCurrentNetwork.Value >= miniGamePlayerMinimum
            && isLaunchingStart == false)
        {
            isLaunchingStart = true;
            MiniGameStarted();
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void DisplayPlayerInMiniGameServerRpc()
    {
        //display the current players' names in text
        string tempPlayerInLobby = "";
        foreach (string name in objectInteractable.interactorNameList)
        {
            tempPlayerInLobby = tempPlayerInLobby + (" ") + name;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void MiniGameVoteStartServerRpc(bool vote)
    {
        print("vote working in client");
        if (vote == true)
        {
            voteList.Add(true);
        }
        else
        {
            if(voteList.Count == 0) { return; }
            voteList.Remove(true);
        }
    }

    private void MiniGameStarted()
    {
        print("game started");
        isMiniGameStarted = true;
    }

    [ServerRpc(RequireOwnership = true)]
    public void MiniGameOverServerRpc()
    {
        // - wait for a moment before reset the minigame
        // - make the players leave the minigame
        // - will be called in other minigame

        Invoke("MiniGameReset", 3f);
        isMiniGameOver = true;
        print("game over");
    }
    private void MiniGameReset()
    {
        // - reset the status and remove players names and votes from the list

        isMiniGameStarted = false;
        isMiniGameOver = false;
        isLaunchingStart = false;
        canPlayerJoinMiniGame = true;
        
        objectInteractable.InteractionCDismissAll();
        miniGameControllerScore.ResetMiniGameScoreServerRpc();

        StartCoroutine(resetVote());

        print("minigame reset");
    }
    IEnumerator resetVote()     
    {
        //the player vote cancle condition is tied to player's transform and the minigame object's transform
        //sending the minigame object to the space and bring back in milisec will do the trick
        voteList.Clear();
        this.transform.position = new Vector3(transform.position.x, transform.position.y + 300f, transform.position.z);
        yield return new WaitForSeconds(0.1f);
        this.transform.position = new Vector3(transform.position.x, transform.position.y - 300f, transform.position.z);
    }

    public void StopMiniGame()
    {
        // - StopMinigame will be called in this function when the game is over by turn, or timeout,
        // - or one of the players unfortunately disconnected, in this condition will kick players off that game.
    }
}