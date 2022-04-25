using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Text;
using UnityEngine.UI;
using System;

public class LogInManager : NetworkBehaviour
{
    public Text playerNameInputField;   //the text get from inputfield
    public GameObject loginPanel;
    public GameObject leaveButton;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HanldeClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

        loginPanel.SetActive(true);
        leaveButton.SetActive(false);
    }


    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) { return; }
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HanldeClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }
    private void HandleServerStarted()
    {
        //throw new NotImplementedException();
    }
    private void HanldeClientConnected(ulong clientId)
    {
        print(playerNameInputField.text + "client id: " + clientId);
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            loginPanel.SetActive(false);
            leaveButton.SetActive(true);
        }

        //throw new NotImplementedException();
    }
    private void HandleClientDisconnect(ulong obj)
    {
        loginPanel.SetActive(true);
        leaveButton.SetActive(false);
    }

    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;   //subscript ConnectionApprovalCallback to approvealCheck
        NetworkManager.Singleton.StartHost();                                   //start Host function
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId,
        NetworkManager.ConnectionApprovedDelegate callback)
    {
        //throw new NotImplementedException();  //show red line debug that the method is functional; 

        string playerName = Encoding.ASCII.GetString(connectionData);   //encode and store the name of previous player joining network.
        bool approveConnection = playerName != playerNameInputField.text;   //ApproveConnetion indicate that you can join the game or not
                                                                            //this where we will put logic argument.
                                                                            //From this example, if player existed player is not the same as
                                                                            //the new player connected in.
                                                                            //so true, and new player can join the game.
                                                                            //In the other hand, unapproved to join.

        if (playerNameInputField.text == "no name")
        {
            print("not approved");
            approveConnection = false;
        }

        print(playerName + "   " + playerNameInputField.text);

        bool createPlayerObject = true; //are you allowed to create your player object
        //bool approveConnection = true; //this is for true test

        callback(createPlayerObject, /*playerPrefabHash*/ null, approveConnection, /*positionSpawnAt*/ null, /*rotationSpawnWith*/ null);
        //this callback is crucial part of spawning player object, where to start, and are you allowed to join or not
        //createPlayerObject (bool): will player object(character) be created?
        //playerPrefabHash (uint): specific prefab index to be created for that player; while set to null, it will use default prefab assigned in MainGameManager which index is 0
        //approval connection (bool): are you approved to join the network? pretty plain really.
        //positionSpawnAt (Vector3): where to spawn player.
        //rotationSpawnWith (Vector3): rotation of player.

        //***fyi: uint is alt. type of int which cannot be negative value.
    }

    public void Client()
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionData =
            Encoding.ASCII.GetBytes(playerNameInputField.text);     //encoding and store player's name value for later checking aprroval
        NetworkManager.Singleton.StartClient();     //start the client

    }
}
