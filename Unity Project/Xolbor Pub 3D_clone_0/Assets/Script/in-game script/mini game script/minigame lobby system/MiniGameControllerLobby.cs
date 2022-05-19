using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(MiniGameControllerScore))]
[RequireComponent(typeof(ObjectInteractable))]

public class MiniGameControllerLobby : NetworkBehaviour
{
    //mini-game joining system
    //server operated script

    //public int miniGamePlayerCurrent;

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

    private StringBuilder stringBuilder;
    private string appendedIdList;
    public string appendedIdListString;

    public List<bool> voteList = new List<bool>();

    public NetworkVariable<int> miniGamePlayerCurrentNetwork = new NetworkVariable<int>();
    public NetworkVariable<int> voteCountNetwork = new NetworkVariable<int>();
    public NetworkVariable<bool> isMiniGameStartedNetwork = new NetworkVariable<bool>();
    public NetworkVariable<bool> isMiniGameOverNetwork = new NetworkVariable<bool>();
    public NetworkVariable<NetworkString> playerIdListAppend = new NetworkVariable<NetworkString>();
    public NetworkVariable<NetworkString> miniGameStatusNetwork = new NetworkVariable<NetworkString>();


    private void Start()
    {
        stringBuilder = new StringBuilder();
        objectInteractable = GetComponent<ObjectInteractable>();
        if (GetComponent<MiniGameControllerScore>() != null)
        {
            miniGameControllerScore = GetComponent<MiniGameControllerScore>();
        }
        InvokeRepeating("CheckMiniGamePlayerAndAppendId", 1f, 1f);
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            CheckMiniGameStatus();
            CheckMiniGameVote();
        }
    }
    private void CheckMiniGameStatus()
    {
        miniGamePlayerCurrentNetwork.Value = objectInteractable.interactorNameList.Count;
        isMiniGameStartedNetwork.Value = isMiniGameStarted;
        isMiniGameOverNetwork.Value = isMiniGameOver;
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
    private void CheckMiniGameVote()
    {
        voteCountNetwork.Value = voteList.Count;
        if (voteCountNetwork.Value == 0 || isMiniGameStarted == true) { return; }


        if (voteCountNetwork.Value == miniGamePlayerCurrentNetwork.Value
            && miniGamePlayerCurrentNetwork.Value >= miniGamePlayerMinimum
            && isLaunchingStart == false)
        {
            isLaunchingStart = true;
            MiniGameStartedServerRpc();
        }
    }
    private void CheckMiniGamePlayerAndAppendId()
    {
        if (!IsServer) { return; }
        if (objectInteractable.interactorIdList.Count == 0)
        {
            playerIdListAppend.Value = "";
        }
        if (objectInteractable.interactorIdList.Count == 1)
        {
            playerIdListAppend.Value = objectInteractable.interactorIdList[0].ToString();
        }
        if (objectInteractable.interactorIdList.Count > 1)
        {
            for (int i = 0; i < objectInteractable.interactorIdList.Count; i++)
            {
                stringBuilder.Append(objectInteractable.interactorIdList[i]);
                if (i < objectInteractable.interactorIdList.Count - 1)
                {
                    stringBuilder.Append(',');
                }
            }
            appendedIdList = stringBuilder.ToString();
            if (appendedIdList != playerIdListAppend.Value)
            {
                playerIdListAppend.Value = stringBuilder.ToString();
            }
            appendedIdList = "";
            stringBuilder.Clear();
        }
        appendedIdListString = playerIdListAppend.Value;
        print(playerIdListAppend.Value);
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
        if (vote == true)
        {
            voteList.Add(true);
            print("vote");
        }
        else
        {
            if (voteList.Count == 0) { return; }
            print("remove a vote");
            voteList.Remove(true);
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void MiniGameStartedServerRpc()
    {
        //the multiplayer and minigame will reference the isMiniGameStarted and will get the player in lobby to minigame

        print("game started");
        isMiniGameStarted = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void MiniGameOverServerRpc()
    {
        // - wait for a moment before reset the minigame
        // - make the players leave the minigame
        // - will be called in other minigame

        isMiniGameOver = true;
        isLaunchingStart = false;
        isMiniGameStarted = false;
        Invoke("MiniGameResetServerRpc", 3f);

        print("game over");
    }
    [ServerRpc(RequireOwnership = false)]
    private void MiniGameResetServerRpc()
    {
        // - reset the status and remove players names and votes from the list

        CancelInvoke("MiniGameResetServerRpc");

        isMiniGameOver = false;
        canPlayerJoinMiniGame = true;

        objectInteractable.InteractionCDismissAll();

        if (miniGameControllerScore != null)    //if not an arcade game, reset the scores in lobby and display winner.
        {
            miniGameControllerScore.ResetMiniGameScoreServerRpc();
        }

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
        StopAllCoroutines();
    }

    //unused
    public void StopMiniGame()
    {
        // - StopMinigame will be called in this function when the game is over by turn, or timeout,
        // - or one of the players unfortunately disconnected, in this condition will kick players off that game.
    }
}