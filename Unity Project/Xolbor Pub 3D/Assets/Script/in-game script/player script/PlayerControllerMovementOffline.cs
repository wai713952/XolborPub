using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerMovementOffline : MonoBehaviour
{
    //the MainPlayer script is attached to player object
    //the player object use ClientNetworkTransform, just know that it's better than typical NetworkTransform
    //created in week4 - netcode introduction(project setup, RPC-method, lauching as host/client/server)

    public float speed = 15.0f;
    public float rotationSpeed = 30f;
    Rigidbody rigidbody;


    private void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        translation *= Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + this.transform.forward * translation);

        float rotation = Input.GetAxis("Horizontal");
        if (rotation != 0)
        {
            rotation *= rotationSpeed;
            Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
            rigidbody.MoveRotation(rigidbody.rotation * turn);
        }
        else
        {
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
}