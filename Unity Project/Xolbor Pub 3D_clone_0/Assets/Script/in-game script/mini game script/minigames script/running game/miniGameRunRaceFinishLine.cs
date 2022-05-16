using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MiniGameRunRaceFinishLine : NetworkBehaviour
{
    public int playerIdInObject;
    GameObject runRaceScript;

    public void FinishLineSetup(int playerId, GameObject runRaceScript)
    {
        playerIdInObject = playerId;
        runRaceScript = runRaceScript;
    }
    private void OnTriggerEnter(Collider player)
    {
        if (player.tag == "Player" && player.GetComponent<PlayerControllerInteraction>().playerId == playerIdInObject)
        {
            runRaceScript.GetComponent<MiniGameRunRace>();
        }
    }
}