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

    public bool isInteractable = true;              // can the player interact with this object (the object might be using by other player)
    public bool isDestroyerAfterInteraction;        // destroy after interaction
    private string objectType;

    public enum ObjectTypeList
    { 
        door,
        food,
        itemGenerator,
        responseItem
    };
    public ObjectTypeList objectTypeList;

    private void Start()
    {
        setObjectType();
    }
    private void setObjectType()
    {
        switch (objectTypeList)
        {
            case ObjectTypeList.door:
                objectType = "door";
                isDestroyerAfterInteraction = false;
                break;

            case ObjectTypeList.food:
                objectType = "food";
                isDestroyerAfterInteraction = true;
                break;

            case ObjectTypeList.itemGenerator:
                objectType = "item generator";
                isDestroyerAfterInteraction = false;
                break;

            case ObjectTypeList.responseItem:
                objectType = "response item";
                isDestroyerAfterInteraction = false;
                break;
        }
    }

    public void InteractedByPlayer(Transform interactorTransform)
    {
        // request interaction to server
        
        switch (objectType)
        {
            case "door":
            {
                GetComponent<ObjectDoor>().TeleportCall(interactorTransform);
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
        DestroyObjectServerRpc();    // destroy the object after interaction (food, drink, garbage)
    }
    [ServerRpc(RequireOwnership = false)]
    private void DestroyObjectServerRpc()
    {
        if (isDestroyerAfterInteraction == true)
        {
            this.gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }
}