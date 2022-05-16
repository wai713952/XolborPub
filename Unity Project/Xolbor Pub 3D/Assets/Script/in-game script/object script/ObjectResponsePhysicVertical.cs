using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ObjectResponsePhysicVertical : NetworkBehaviour
{
    //when pressed will moving by force added to rigidbody
    //has reset time to be interact again

    public NetworkVariable<bool> canBeInteractWith = new NetworkVariable<bool>();

    public float interactionPhysicForce;
    public float interactionPhysicTorque;

    public float forceAndTorqueVariant;
    private float interactionPhysicForceRandom;
    private float interactionPhysicTorqueRandom;

    public float waitToSpinTime;
    public float interactionResetTime;
    public GameObject childRespondObject;

    private void Start()
    {
        canBeInteractWith.Value = true;
        transform.position = new Vector3(childRespondObject.transform.position.x,
            transform.position.y, childRespondObject.transform.position.z);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResponseToInteractServerRpc()
    {
        
        if (canBeInteractWith.Value == true)
        {
            canBeInteractWith.Value = false;
            if (interactionPhysicTorque != 0)
            {
                Invoke("WaitToSpin", waitToSpinTime);
            }

            interactionPhysicForceRandom = Random.Range(interactionPhysicForce - forceAndTorqueVariant, interactionPhysicForce);
            transform.GetChild(0).GetComponent<Rigidbody>().AddForce(Vector3.up * interactionPhysicForceRandom);

            Invoke("ResetInteractionAfterPhysic", interactionResetTime);
        }
    }
    private void WaitToSpin()
    {
        interactionPhysicTorqueRandom = Random.Range(interactionPhysicTorque - forceAndTorqueVariant, interactionPhysicTorque + forceAndTorqueVariant);
        transform.GetChild(0).GetComponent<Rigidbody>().AddTorque(Vector3.forward * interactionPhysicTorqueRandom);
    }

    private void ResetInteractionAfterPhysic()
    {
        canBeInteractWith.Value = true;
    }
}
