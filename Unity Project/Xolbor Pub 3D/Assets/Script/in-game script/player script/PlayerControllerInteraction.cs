using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerControllerInteraction : NetworkBehaviour
{
    private Ray interactionRay;
    private RaycastHit interactionHitObject;
    public LayerMask interactionMask;
    public float interactionRange = 250f;

    //**testing network variable
    public string playerName;   //this will later reference to player's name in MainPlayer script

    private void Update()
    {
        if (IsClient && IsOwner)
        {
            PlayerRayCast();
        }
    }
    private void PlayerRayCast()
    {
        interactionRay = new Ray(transform.position, transform.forward); //declare a ray
        if (interactionRange > 0) //when ray's range is more then zero
        {
            if (Physics.Raycast(interactionRay, out interactionHitObject, interactionRange, 
                interactionMask, QueryTriggerInteraction.Collide))                              //when the ray hit on specific mask(s)
            {
                playerName = "jack";
                GameObject tempInteractableObject = interactionHitObject.collider.gameObject;
                PlayerInteract(tempInteractableObject, playerName);
                Debug.DrawLine(interactionRay.origin, interactionHitObject.point, Color.red);   //show the red ray
            }
            else //when not hitting in specfied layer
            {
                Debug.DrawLine(interactionRay.origin, interactionRay.origin + 
                    interactionRay.direction * interactionRange, Color.green);  //show the green line
            }
        }
    }
    private void PlayerInteract(GameObject interactableObject, string playerName)
    {
        string tempPlayerName = playerName;
        int tempPlayerId = 0;
        Transform tempPlayerTransform = this.transform;
        if (Input.GetKeyDown(KeyCode.E))
        {
            interactableObject.gameObject.GetComponent<ObjectInteractable>().InteractedByPlayer(tempPlayerName, tempPlayerId, tempPlayerTransform);
        }
    }
    
}
