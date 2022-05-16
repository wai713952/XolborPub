using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ObjectInteractable : NetworkBehaviour
{
    // - interactable object can be interacted by pressing E
    // - in some condition player cannot interact with certain object by this condition:
    //      1.currently interacted by other player
    //      2.not in the state that ready to interact
    //      3.player's priority
    // - these following condition will passing through the InteractedByPlayer's parameters
    // - also existed in every client, but excuted by requesting to server

    public bool isInteractable = true;              //can the player interact with this object (the object might be using by other player)
    public bool isInteractedByLocalPlayer;          //is the local player currently interacting with the object
    public bool isEnabledMultipleInteraction;       //object can be interact by more than 1 player at time, like multiplayer minigames
    public bool isDestroyerAfterInteraction;        //destroy after interaction
    private string objectType;

    public int pressCount = 0;
    public bool isEnabledSecondPress = false;

    public enum ObjectTypeList
    { 
        miniGameMulti,
        miniGameSingle,
        door,
        food,
        itemGenerator,
        responseItem
    };
    public ObjectTypeList objectTypeList;

    public List<string> interactorNameList = new List<string>();            //while multiple interaction is true, store player names
    public List<int> interactorIdList = new List<int>();

    private void Start()
    {
        setObjectType();
    }
    private void setObjectType()
    {
        switch (objectTypeList)
        {
            case ObjectTypeList.miniGameSingle:
                objectType = "minigame single-player";
                isDestroyerAfterInteraction = false;
                isEnabledMultipleInteraction = true;
                break;

            case ObjectTypeList.miniGameMulti:
                objectType = "minigame multiplayer";
                isDestroyerAfterInteraction = false;
                isEnabledMultipleInteraction = true;
                isEnabledSecondPress = true;
                break;

            case ObjectTypeList.door:
                objectType = "door";
                isDestroyerAfterInteraction = false;
                isEnabledMultipleInteraction = false;
                break;

            case ObjectTypeList.food:
                objectType = "food";
                isDestroyerAfterInteraction = true;
                isEnabledMultipleInteraction = false;
                break;

            case ObjectTypeList.itemGenerator:
                objectType = "item generator";
                isDestroyerAfterInteraction = false;
                isEnabledMultipleInteraction = false;
                break;

            case ObjectTypeList.responseItem:
                objectType = "response item";
                isDestroyerAfterInteraction = false;
                isEnabledMultipleInteraction = false;
                break;
        }
    }

    public void InteractedByPlayer(string interactorName, int interactorId, Transform interactorTransform)
    {
        // - request interaction to server
        
        switch (objectType)
        {
            case "minigame single-player":
            {
                MiniGameControllerLobby lobby = GetComponent<MiniGameControllerLobby>();
                if (lobby.miniGamePlayerCurrentNetwork.Value <= lobby.miniGamePlayerMaximum)
                {
                    if (pressCount == 0)    //pressing first time will join the game
                    {
                        print("first press");
                        pressCount++;
                        AddPlayerToInteractorListServerRpc(interactorName, interactorId);
                        return; 
                    }
                    if (pressCount == 1)     //pressing second time will vote the to start
                    {
                        print("second press");
                        pressCount++;
                        lobby.MiniGameVoteStartServerRpc(true);
                    }
                }
                break;
            }
            case "minigame multiplayer":
            {
                //GetComponent<MiniGameControllerLobby>();
                //AddPlayerToInteractorList(interactorName);
                break;
            }
            case "door":
            {
                GetComponent<ObjectDoor>().Teleport(interactorTransform);
                break;
            }
            case "food":
            {
                
                break;
            }
            case "item generator":
            {
                GetComponent<ObjectCreator>().CreateItem();
                break;
                }
            case "response item":
                {
                    if (GetComponent<ObjectResponsePhysicVertical>() != null)
                    {
                        GetComponent<ObjectResponsePhysicVertical>().ResponseToInteractServerRpc();
                    }
                    if (GetComponent<ObjectResponsePhysicHorizontal>() != null)
                    {
                        Vector3 playerPosition = new Vector3(interactorTransform.position.x, interactorTransform.position.y, interactorTransform.position.z);
                        GetComponent<ObjectResponsePhysicHorizontal>().ResponseToInteractServerRpc(playerPosition);
                    }
                    if (GetComponent<ObjectResponseAnimation>() != null)
                    {
                        GetComponent<ObjectResponseAnimation>().ResponseToInteractServerRpc();
                    }

                    break;
                }

        }
        DestroyObjectServerRpc();    //destroy the object after interaction (food, drink, garbage)
    }
    [ServerRpc(RequireOwnership = false)]
    private void DestroyObjectServerRpc()
    {
        if (isDestroyerAfterInteraction == true)
        {
            this.gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPlayerToInteractorListServerRpc(string interactorName, int interactorId)
    {
        if (isEnabledMultipleInteraction == true)
        {
            for (int i = 0; i < interactorNameList.Count; i++)
            {
                if (interactorName == interactorNameList[i]) { return; }
            }
            interactorNameList.Add(interactorName);
            interactorIdList.Add(interactorId);
        }
    }
    
    public Transform SendPlayerObjectPosition()
    {
        return this.transform;
    }

    public void RemovePlayerNameAndVoteFromList(string playerName)
    {
        RemovePlayerNameFromListServerRpc(playerName);
        if (pressCount == 2)
        {
            RemovePlayerVoteFromListServerRpc();
        }

    }
    [ServerRpc(RequireOwnership = false)]
    private void RemovePlayerNameFromListServerRpc(string removedPlayer)
    {
        // - quit the interaction
        // - if there's mini-game lobby control, also remove player from lobby (interactors list will be reference in mini-game lobby script)
        // - player can quit the object interaction by simply walking out of interaction area (trigger)
        // - stay too far too, make player automatically leave the interaction point
        if (interactorNameList.Count == 0) { return; }

        for (int i = 0; i < interactorNameList.Count; i++)
        {
            if (removedPlayer == interactorNameList[i])
            {
                interactorNameList.Remove(interactorNameList[i]);
                interactorIdList.Remove(interactorIdList[i]);
                print("get in isclient");
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void RemovePlayerVoteFromListServerRpc()
    {
        print("remove vote");
        MiniGameControllerLobby lobby = GetComponent<MiniGameControllerLobby>();    //in case the player has already voted start and leave the game
        lobby.MiniGameVoteStartServerRpc(false);                                    //don't forget to clear the vote for that player
    }

    public void InteractionCDismissAll()
    {
        interactorNameList.Clear();
        interactorIdList.Clear();
    }
}