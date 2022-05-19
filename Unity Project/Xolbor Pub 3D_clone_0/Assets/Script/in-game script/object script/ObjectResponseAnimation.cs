using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ObjectResponseAnimation : NetworkBehaviour
{
    //when pressing to some object, it can react to the player by playing animation, or moving around
    //must have network animation
    
    NetworkVariable<bool> canBeInteractWith = new NetworkVariable<bool>();

    Animator animator;
    private float timeWaitForNextInteraction = 1f;

    private void Start()
    {
        canBeInteractWith.Value = true;
        animator = GetComponent<Animator>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResponseToInteractServerRpc()
    {
        if (canBeInteractWith.Value == true)
        {
            canBeInteractWith.Value = false;
            animator.SetBool("play animation", true);
        }
    }

    public void ResetInteractionAfterAnimation()
    {
        //this method will be called in animation frame

        animator.SetBool("play animation", false);
        Invoke("ResetWaiting", timeWaitForNextInteraction);
    }
    private void ResetWaiting()
    {
        canBeInteractWith.Value = true;
    }
}
