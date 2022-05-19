using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.IO;
using System;

public class MiniGameBasketball : NetworkBehaviour
{
    MiniGameControllerLobby lobby;
    MiniGameControllerArcade arcade;

    public GameObject scoreText;
    public GameObject winnerText;

    public NetworkVariable<float> basketballTimeCurrentNetwork = new NetworkVariable<float>();
    public float basketballTimeCurrent;
    public float basketballTimeMax;

    public NetworkVariable<bool> startMiniGameBasketballNetwork = new NetworkVariable<bool>();
    public bool startMiniGameBasketball = false;

    private void Start()
    {
        basketballTimeCurrent = 0f;
        lobby = GetComponent<MiniGameControllerLobby>();
        arcade = GetComponent<MiniGameControllerArcade>();
    }

    private void Update()
    {
        print("isclient");
        if (lobby.isMiniGameStartedNetwork.Value == true && lobby.isMiniGameOverNetwork.Value == false && startMiniGameBasketballNetwork.Value == false)
        {
            print("basketball started");

            string idAppended;
            string[] idSplitList;
            int idFromListConvertedToInt;
            int idFromLocalConvertedToInt;

            idFromLocalConvertedToInt = Convert.ToInt32(NetworkManager.Singleton.LocalClientId);
            idAppended = lobby.playerIdListAppend.Value;
            idSplitList = idAppended.Split(',');

            if(idSplitList.Length == 0) { return; }
            for (int i = 0; i < idSplitList.Length; i++)
            {
                int.TryParse(idSplitList[i], out idFromListConvertedToInt);
                if (idFromListConvertedToInt == idFromLocalConvertedToInt)
                {
                    print("set time");
                    basketballTimeCurrent = basketballTimeMax;

                    SetTimeServerRpc();
                    StartBasketBallGameServerRpc();
                }
            }
        }

        if (IsServer)
        {
            basketballTimeCurrentNetwork.Value = basketballTimeCurrent;
        }
        

        if (basketballTimeCurrentNetwork.Value > 0 && startMiniGameBasketballNetwork.Value == true && lobby.isMiniGameStartedNetwork.Value == true)
        {
            basketballTimeCurrent -= Time.deltaTime;
        }
        if (basketballTimeCurrentNetwork.Value < 0 && startMiniGameBasketballNetwork.Value == true && lobby.isMiniGameOverNetwork.Value == false)
        {
            ShowWinnerServerRpc();
        }
    }

    
    [ServerRpc(RequireOwnership = false)]
    private void SetTimeServerRpc()
    {
        basketballTimeCurrent = basketballTimeMax;
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartBasketBallGameServerRpc()
    {
        startMiniGameBasketball = true;
        startMiniGameBasketballNetwork.Value = startMiniGameBasketball;


        print("basketball UI created");

        GameObject createdText = Instantiate(scoreText, scoreText.transform.position, scoreText.transform.rotation);
        createdText.transform.SetParent(GameObject.FindGameObjectWithTag("MainCanvas").transform);
    }
    [ServerRpc(RequireOwnership = false)]
    private void ShowWinnerServerRpc()
    {
        startMiniGameBasketball = false;
        startMiniGameBasketballNetwork.Value = startMiniGameBasketball;

        ShowWinnerClientRpc();

        lobby.MiniGameOverServerRpc();
    }
    [ClientRpc]
    private void ShowWinnerClientRpc()
    {
        GameObject createdText = Instantiate(winnerText, winnerText.transform.position, winnerText.transform.rotation);
        createdText.GetComponent<TMP_Text>().text = "winner";
    }
}
