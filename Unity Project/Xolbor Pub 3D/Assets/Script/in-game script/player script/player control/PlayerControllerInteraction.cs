using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerControllerInteraction : NetworkBehaviour
{
    public bool canControlCharacterRay = true;

    private Ray interactionRay;
    private RaycastHit interactionHitObject;
    public LayerMask interactionMask;
    public float interactionRange = 250f;
    public float interactionHeight;
    
    private MainPlayer mainPlayer;

    public string playerName;
    public int playerId;
    
   
    private void Start()
    {
        Invoke("NameAndIdGet", 1f);
    }
    private void NameAndIdGet()
    {
        mainPlayer = GetComponent<MainPlayer>();
        playerName = mainPlayer.PlayerName.Value;

        playerId = Convert.ToInt32(NetworkManager.Singleton.LocalClientId);
    }

    private void Update()
    {
        if (IsClient && IsOwner)
        {
            if(canControlCharacterRay == false) { return; }
            PlayerRayCast();
        }
    }
    private void PlayerRayCast()
    {
        interactionRay = new Ray(transform.position + Vector3.up * interactionHeight, transform.forward); //declare a ray
        if (interactionRange > 0) //when ray's range is more then zero
        {
            if (Physics.Raycast(interactionRay, out interactionHitObject, interactionRange, 
                interactionMask, QueryTriggerInteraction.Collide))                              //when the ray hit on specific mask(s)
            {
                GameObject tempInteractableObject = interactionHitObject.collider.gameObject;
                PlayerInteract(tempInteractableObject);

                Debug.DrawLine(interactionRay.origin, interactionHitObject.point, Color.red);   //show the red ray
            }
            else //when not hitting in specfied layer
            {
                Debug.DrawLine(interactionRay.origin, interactionRay.origin + 
                    interactionRay.direction * interactionRange, Color.green);  //show the green line
            }
        }
    }

    private void PlayerInteract(GameObject interactableObject)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            print("player interact");
            ObjectInteractable interact = interactableObject.GetComponent<ObjectInteractable>();
            interact.InteractedByPlayer(transform);
        }
    }
}
