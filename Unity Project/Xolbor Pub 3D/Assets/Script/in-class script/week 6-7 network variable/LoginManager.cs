using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Text;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public Text playerNameInputField;   //the text get from inputfield
    public GameObject loginPanel;
    public GameObject leaveButton;
    public GameObject scorePanel;

    ObjectJukebox objectJukebox;

    private void Start()    //subscribe the event
    {
        SetNetworkHandle();
        SetJukebox();
        SetUiVisibility(false);
    }
    private void SetNetworkHandle()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }
    private void SetJukebox()
    {
        objectJukebox = GameObject.FindWithTag("Jukebox").GetComponent<ObjectJukebox>();

        NetworkManager.Singleton.OnServerStarted += objectJukebox.GetComponent<ObjectJukebox>().StartSong;
        NetworkManager.Singleton.OnClientConnectedCallback += objectJukebox.GetComponent<ObjectJukebox>().StartSongClient;
    }
    private void SetUiVisibility(bool isUserLogin)
    {
        if (isUserLogin)
        {
            loginPanel.SetActive(false);
            leaveButton.SetActive(true);
            scorePanel.SetActive(true);
        }
        else
        {
            loginPanel.SetActive(true);
            leaveButton.SetActive(false);
            scorePanel.SetActive(false);
        }
    }

    private void OnDestroy()    //destroyed, unsubscribe the event; prevent adverse effect
    {
        if (NetworkManager.Singleton == null) { return; }
        RemoveNetworkHandle();
        RemoveJukebox();
    }
    private void RemoveNetworkHandle()
    {
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }
    private void RemoveJukebox()
    {
        NetworkManager.Singleton.OnServerStarted -= objectJukebox.GetComponent<ObjectJukebox>().StartSong;
        NetworkManager.Singleton.OnClientConnectedCallback -= objectJukebox.GetComponent<ObjectJukebox>().StartSongClient;
    }

    private void HandleClientConnected(ulong clientId)      //when client connected
    {
        Debug.Log("client id = " + clientId);
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            SetUiVisibility(true);
        }
    }

    private void HandleClientDisconnect(ulong clientId)     //when client disconected
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            SetUiVisibility(false);
        }
    }

    private void HandleServerStarted()          //when server started
    {
        throw new NotImplementedException();    //keep working even the fucntion is not yet implemented
    }

    public void Host()  //login as host button
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
        string[] unapprovedName = { "", " ", "a", "asdf" };
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
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }
        SetUiVisibility(false);
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId,
        NetworkManager.ConnectionApprovedDelegate callback)
    {
        string playerName = Encoding.ASCII.GetString(connectionData);       //encode and store the name of previous player joining network.
        bool approveConnection = playerName != playerNameInputField.text;   // = playerName != playerNameInputField.text;   //ApproveConnetion indicate that you can join the game or not
                                                                            //this is where we will put logic argument.
                                                                            //From this example, if existed player's name is not the same as
                                                                            //the new player connected in.
                                                                            //if true, and new player can join the game.
                                                                            //In the other hand, unapproved to join.

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        // if it's server player
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            spawnPos = new Vector3(-2f, 1f, 0f);
            spawnRot = Quaternion.Euler(0f, 135f, 0f);
        }
        else
        {
            switch (NetworkManager.Singleton.ConnectedClients.Count)
            {
                case 1:
                    spawnPos = new Vector3(0f, 1f, 0f);
                    spawnRot = Quaternion.Euler(0f, 180f, 0f);
                    break;
                case 2:
                    spawnPos = new Vector3(2f, 1f, 0f);
                    spawnRot = Quaternion.Euler(0f, 225f, 0f);
                    break;
            }
        }

        NetworkLog.LogInfoServer("SpawnPos of " + clientId + " is " + spawnPos.ToString());

        //**approveConnection = true; //this is for true test
        bool createPlayerObject = true;

        callback(createPlayerObject, null, approveConnection, spawnPos, null);
        //this callback is crucial part of spawning player object, where to start, and are you allowed to join or not
        //createPlayerObject (bool): will player object(character) be created?
        //playerPrefabHash (uint): specific prefab index to be created for that player; while set to null, it will use default prefab assigned in MainGameManager which index is 0
        //approval connection (bool): are you approved to join the network? pretty plain really.
        //positionSpawnAt (Vector3): where to spawn player.
        //rotationSpawnWith (Vector3): rotation of player.

        //***fyi: uint is alt. type of int which cannot be negative value.
    }

}
