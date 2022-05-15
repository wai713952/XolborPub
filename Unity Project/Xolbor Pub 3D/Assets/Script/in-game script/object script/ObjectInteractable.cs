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
        itemGenerator
    };
    public ObjectTypeList objectTypeList;

    public List<string> interactorNameList = new List<string>();            //while multiple interaction is true, store player names
    public List<int> interactorIdList = new List<int>();
    public List<Transform> interactorTransformList = new List<Transform>();

    private void Start()
    {
        setObjectType();
        InvokeRepeating("InteractionDismiss", 0.5f, 1f);
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
                if (lobby.miniGamePlayerCurrent <= lobby.miniGamePlayerMaximum)
                {
                    if (pressCount == 0)    //pressing first time will join the game
                    {
                        AddPlayerToInteractorList(interactorName, interactorId, interactorTransform);
                        pressCount++;
                        return; 
                    }
                    if (pressCount == 1)     //pressing second time will vote the to start
                    {
                        lobby.MiniGameVoteStart(true);
                        pressCount++;
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

    private void AddPlayerToInteractorList(string interactorName, int interactorId, Transform interactorTransform)
    {
        if (isEnabledMultipleInteraction == true)
        {
            for (int i = 0; i < interactorNameList.Count; i++)
            {
                if (interactorName == interactorNameList[i]) { return; }
            }
            interactorNameList.Add(interactorName);
            interactorIdList.Add(interactorId);
            interactorTransformList.Add(interactorTransform);
        }
    }

    private void InteractionDismiss()
    {
        // - quit the interaction
        // - if there's mini-game lobby control, also remove player from lobby (interactors list will be reference in mini-game lobby script)
        // - player can quit the object interaction by simply walking out of interaction area (trigger)
        // - stay too far too, make player automatically leave the interaction point
        if (interactorNameList.Count == 0) { return; }

        for (int i = 0; i < interactorNameList.Count; i++)
        {
            if (Vector3.Distance(transform.position, interactorTransformList[i].position) > 2f)
            {
                interactorNameList.Remove(interactorNameList[i]);
                interactorIdList.Remove(interactorIdList[i]);
                interactorTransformList.Remove(interactorTransformList[i]);
                pressCount = 0;

                MiniGameControllerLobby lobby = GetComponent<MiniGameControllerLobby>();
                lobby.MiniGameVoteStart(false);
            }
        }
    }
    public void InteractionCDismissAll()
    {
        interactorNameList.Clear();
        interactorIdList.Clear();
        interactorTransformList.Clear();
    }
}