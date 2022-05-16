using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MiniGameControllerScore : NetworkBehaviour
{
    public List<string> miniGamePlayerNameArray = new List<string>();
    public List<float> miniGamePlayerScoreArray = new List<float>();
    public List<int> miniGamePlayerClientIdArray = new List<int>();

    MiniGameControllerLobby miniGameControllerLobby;

    void Start()
    {
        miniGameControllerLobby = GetComponent<MiniGameControllerLobby>();    
    }

    [ServerRpc(RequireOwnership = false)]
    public void RecieveMiniGameScoreServerRpc(string playerName, float playerScore, int playerClientId)
    {
        // - recieving string of player's name and score from client
        // - called from client everytime their score has been created
        miniGamePlayerNameArray.Add(playerName);
        miniGamePlayerScoreArray.Add(playerScore);
        miniGamePlayerClientIdArray.Add(playerClientId);

        if (miniGamePlayerNameArray.Count > miniGameControllerLobby.miniGamePlayerCurrentNetwork.Value 
            && miniGamePlayerScoreArray.Count > miniGameControllerLobby.miniGamePlayerCurrentNetwork.Value)
        {
            for (int i = 0; i < miniGameControllerLobby.miniGamePlayerCurrentNetwork.Value; i++)
            {
                if (playerName == miniGamePlayerNameArray[i])
                {
                    miniGamePlayerNameArray.RemoveAt(i);
                    miniGamePlayerScoreArray.RemoveAt(i);
                    miniGamePlayerClientIdArray.RemoveAt(i);
                    break;
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = true)]
    public void EvaluateMiniGameScoreServerRpc()
    {
        // - sorting to find the best score index. return best score, score's index and player name with same index
        // - return string array that contains winner's name and score
        // - will execute from the server once the game is over

        //sorting to find best score
        float scoreBestValue = 0;
        int scoreBestIndex = -1;
        int winnerClientId = 99;

        for (int i = 0; i < miniGamePlayerNameArray.Count; i++)
        {
            if (miniGamePlayerScoreArray[i] >= scoreBestValue)
            {
                scoreBestValue = miniGamePlayerScoreArray[i];
                scoreBestIndex = i;
                winnerClientId = miniGamePlayerClientIdArray[i];
            }
        }

        string[] miniGamerWinnerNameAndScore = { miniGamePlayerNameArray[scoreBestIndex], scoreBestValue.ToString() };
        print($"winner: {miniGamerWinnerNameAndScore[0]}___score: {miniGamerWinnerNameAndScore[1]}___id: {winnerClientId.ToString()}");
    }

    [ServerRpc(RequireOwnership = true)]
    public void ResetMiniGameScoreServerRpc()
    {
        miniGamePlayerNameArray.Clear();
        miniGamePlayerScoreArray.Clear();
        miniGamePlayerClientIdArray.Clear();
    }
}
