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
            animator.SetTrigger("play animation");
        }
    }

    public void ResetInteractionAfterAnimation()
    {
        //this method will be called in animation frame
        canBeInteractWith.Value = true;
    }
}