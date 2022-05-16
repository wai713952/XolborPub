using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ObjectResponsePhysicHorizontal : NetworkBehaviour
{
    //
    //after time is reset the interaction object will move the same position as

    public NetworkVariable<bool> canBeInteractWith = new NetworkVariable<bool>();
    public float interactionPhysicForce;

    public float forceAndTorqueVariant;
    private float interactionPhysicForceRandom;

    public float waitToSpinTime;
    public float interactionResetTime;
    public GameObject childRespondObject;

    private void Start()
    {
        canBeInteractWith.Value = true;
        InvokeRepeating("TrackingChildPosition", 1f ,1f);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResponseToInteractServerRpc(Vector3 playerPosition)
    {
        if (canBeInteractWith.Value == true)
        {
            canBeInteractWith.Value = false;

            interactionPhysicForceRandom = Random.Range(interactionPhysicForce - forceAndTorqueVariant, interactionPhysicForce);
            Vector3 newVector = childRespondObject.transform.position - playerPosition;
            childRespondObject.GetComponent<Rigidbody>().AddForce(newVector * interactionPhysicForceRandom);

            Invoke("ResetInteractionAfterPhysic", interactionResetTime);
        }
    }

    private void TrackingChildPosition()
    {
        transform.position = new Vector3(childRespondObject.transform.position.x,
            transform.position.y, childRespondObject.transform.position.z);
    }

    private void ResetInteractionAfterPhysic()
    {
        //this method will be called in animation frame
        canBeInteractWith.Value = true;
        transform.position = new Vector3(childRespondObject.transform.position.x,
            transform.position.y, childRespondObject.transform.position.z);
    }
}
