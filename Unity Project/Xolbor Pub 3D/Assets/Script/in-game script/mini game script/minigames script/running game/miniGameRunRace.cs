using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MiniGameRunRace : NetworkBehaviour
{
    MiniGameControllerLobby lobby;
    MiniGameControllerScore score;
    PlayerControllerInteraction interaction;

    public float raceTime;
    public GameObject gameCamera;
    public GameObject finishLinePrefab;
    public GameObject finishLineLocation;

    public bool startRunRaceGame = false;

    private void Start()
    {
        lobby = GetComponent<MiniGameControllerLobby>();
        score = GetComponent<MiniGameControllerScore>();
        interaction = GetComponent<PlayerControllerInteraction>();
    }

    [ClientRpc]
    public void CreateFinishLineClientRpc()
    {
        //create the finish line for every client, you can only hit your finish line that has same client id as yours stored in it
        //finish line object will hold another script that link to this script

        GameObject finishLine = Instantiate(finishLinePrefab, finishLineLocation.transform.position, finishLineLocation.transform.rotation);
        finishLine.GetComponent<MiniGameRunRaceFinishLine>().FinishLineSetup(interaction.playerId, this.gameObject);
    }

    private void Update()
    {
        if (IsServer)
        {
            CheckLobbyStart();
        }
        if (IsClient && IsOwner)
        {
            TimeWatch();

        }
    }
    private void CheckLobbyStart()
    {
        
    }
    private void TimeWatch()
    {
        //will be call every frame
        raceTime += Time.deltaTime;
    }

    private void GameStartCheck()
    {
        if (lobby.isMiniGameStarted && startRunRaceGame == false)
        {
            startRunRaceGame = true;
            gameCamera.SetActive(true);
        }
    }
    [ServerRpc(RequireOwnership = true)]
    private void RunRaceEndGameServerRpc()
    {
        score.EvaluateMiniGameScoreServerRpc();
        lobby.MiniGameOverServerRpc();
        gameCamera.SetActive(false);
    }

    private void TimeToScore()
    {
        //turn the time into negative, the score sort will change back to positive value
        raceTime = -raceTime;
        score.RecieveMiniGameScoreServerRpc(interaction.playerName, raceTime, interaction.playerId);
    }
   
}
