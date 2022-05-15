using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Text;
using UnityEngine.UI;
using TMPro;
using System;

public class LoginManager : NetworkBehaviour
{
    public Text playerNameInputField;   //the text get from inputfield
    public TMP_Text playerNameInputFieldTMP;
    public GameObject loginPanel;
    public GameObject leaveButton;

    private void Start()                                //subscribe the event
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HanldeClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

        loginPanel.SetActive(true);
        leaveButton.SetActive(false);
    }

    private void OnDestroy()                            //destroyed, unsubscribe the event; prevent adverse effect
    {
        if (NetworkManager.Singleton == null) { return; }
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HanldeClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }
    private void HandleServerStarted()                  //when server started
    {
        throw new NotImplementedException();            //keep working even the fucntion is not yet implemented
    }
    private void HanldeClientConnected(ulong clientId)  //when client connected
    {
        print($"player name: {playerNameInputField.text}  |  client id: {clientId}");
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            loginPanel.SetActive(false);
            leaveButton.SetActive(true);
        }
    }
    private void HandleClientDisconnect(ulong clientId)      //when client disconected
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            loginPanel.SetActive(true);
            leaveButton.SetActive(false);
        }
    }

    public void Host()      //login as host button
    {
        if (LoginNameCheck() == false) { return; }

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;   //subscript ConnectionApprovalCallback to approvealCheck
        NetworkManager.Singleton.StartHost();                                   //start Host function
    }
    public void Client()    //login as client button
    {
        if (LoginNameCheck() == false) { return; }

        NetworkManager.Singleton.NetworkConfig.ConnectionData =
            Encoding.ASCII.GetBytes(playerNameInputField.text);     //encoding and store player's name value for later checking aprroval
        NetworkManager.Singleton.StartClient();                     //start the client
    }
    private bool LoginNameCheck()
    {
        bool isNameApproved = true;
        string[] unapprovedName = {"", " ", "a", "asdf"};
        for (int count = 0; count < unapprovedName.Length; count++)
        {
            if (playerNameInputField.text == unapprovedName[count])
            {
                print("not allowed name");
                isNameApproved = false;
                break;
            }
        }
        return isNameApproved;
    }
    public void Leave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
        if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }
        loginPanel.SetActive(true);
        leaveButton.SetActive(false);
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId,       
        NetworkManager.ConnectionApprovedDelegate callback)
    {
        //throw new NotImplementedException();  //hide red line debug that the method is functional; 

        string playerName = Encoding.ASCII.GetString(connectionData);   //encode and store the name of previous player joining network.
        bool approveConnection = false; // = playerName != playerNameInputField.text;   //ApproveConnetion indicate that you can join the game or not
                                        //this is where we will put logic argument.
                                        //From this example, if existed player's name is not the same as
                                        //the new player connected in.
                                        //if true, and new player can join the game.
                                        //In the other hand, unapproved to join.
        print($"{playerName}  {playerNameInputField.text.ToString()}");
        if (playerName != playerNameInputField.text)
        {
            approveConnection = true;
        }

        Vector3 spawnPosisition = Vector3.zero;

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            spawnPosisition = new Vector3(2f, 1f, 0f);
        }
        else
        {
            switch (NetworkManager.Singleton.ConnectedClients.Count)
            {
                case 1:
                    spawnPosisition = new Vector3(0f, 0f, 0f);
                    break;
                case 2:
                    spawnPosisition = new Vector3(1f, 3f, 1f);
                    break;
            }
        }

        NetworkLog.LogInfoServer("Spawn position of" + clientId + "is" + spawnPosisition.ToString());
        

        bool createPlayerObject = true; //are you allowed to create your player object
        //**approveConnection = true; //this is for true test

        callback(createPlayerObject, /*playerPrefabHash*/ null, approveConnection, /*positionSpawnAt*/ spawnPosisition, /*rotationSpawnWith*/ null);
        
        //this callback is crucial part of spawning player object, where to start, and are you allowed to join or not
        //createPlayerObject (bool): will player object(character) be created?
        //playerPrefabHash (uint): specific prefab index to be created for that player; while set to null, it will use default prefab assigned in MainGameManager which index is 0
        //approval connection (bool): are you approved to join the network? pretty plain really.
        //positionSpawnAt (Vector3): where to spawn player.
        //rotationSpawnWith (Vector3): rotation of player.

        //***fyi: uint is alt. type of int which cannot be negative value.
    }

}
