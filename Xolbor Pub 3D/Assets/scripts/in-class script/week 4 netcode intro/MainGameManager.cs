using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MainGameManager : NetworkBehaviour
{
    //MainGameManger is attached to NetworkManger, its duty is to control the online system.

    //created in week4 - netcode introduction(project setup, RPC-method, lauching as host/client/server)

    void OnGUI()    //control functional buttons created by script
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));    //create area for buttons
        if(!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) 
            //is current player not the client? and...
            //is current player not the server?
        {
            StartButtons(); //if true, show the start button, everytime enter the game,
                            //it will show buttons that ask: will you start as host, client, or server?
        }
        else
        {
            StatusLabels(); //after choose one of three, it will display that you are
                            //a host, client, or server
            SubmitNewPosition(); //also showing move or request to move; depend on what you are
        }

        GUILayout.EndArea(); //close the division area of buttons
    }

    static void StartButtons() //the buttons ask you to choose to be a host, client, or server
    {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();     //start game as the host
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient(); //...as the client
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer(); //...as the server
    }

    static void StatusLabels()  //the status shown that you are a host, client, or server after choosing one of them
    {
        var mode = NetworkManager.Singleton.IsHost ?    //the mode you chose (host/client/server)
                                                        //first it will check that you are a host or not.
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";   //if not, are you a server? if not you are a client.

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);    //the type of transport you're using, not to bother really.
        GUILayout.Label("Mode " + mode);    //display the mode from condition in previous lines
    }

    static void SubmitNewPosition() //a button that moves the player to new position when pressed
    {
        if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request position change"))   //draw a button, if you're server; the butto says move,
                                                                                                        //if not; request postion change
        {
            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();    //refer the player object of that local
            var player = playerObject.GetComponent<MainPlayer>();   //reference script MainPlayer
            player.Move();  //move the player using Move method in MainPlayer
        }
    }
}
