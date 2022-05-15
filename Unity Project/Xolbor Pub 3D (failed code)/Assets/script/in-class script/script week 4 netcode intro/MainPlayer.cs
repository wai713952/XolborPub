using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MainPlayer: NetworkBehaviour
{
    //the MainPlayer script is attached to player object
    //the player object use ClientNetworkTransform, just know that it's better than typical NetworkTransform
    //created in week4 - netcode introduction(project setup, RPC-method, lauching as host/client/server)

    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();  //the position saved after random, and will be used later.

    public float speed = 15f;
    public float rotateSpeed = 30f;
    Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = this.gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotateSpeed;

        translation *= Time.deltaTime;
        Quaternion turn = Quaternion.Euler(0, rotation, 0);

        rigidbody.MovePosition(rigidbody.position + this.transform.forward * translation);
        rigidbody.MoveRotation(rigidbody.rotation * turn);
    }

    public override void OnNetworkSpawn()   //overriding OnNetworkSpawn to execute Move() after conneting to network
    {
        Move(); //move the player to random pose
    }

    public void Move()  //already explained that
    {
        if (NetworkManager.Singleton.IsServer)  //are you the server? if yes, random postion and move you(server) to that place immediately
        {
            var randomPosition = GetRandomPostionOnPlanet();    //random the postion
            transform.position = randomPosition;    //change postion
            Position.Value = randomPosition;    //store the position in vector3

            print("move server");
        }
        else
        {
            SubmitPositionRequestServerRpc();   //if you are the client, you need ask/request the server to move you to new position
        }
    }

    [ServerRpc] //requesting the server
    void SubmitPositionRequestServerRpc()    //ServerRpcParams is an optional attribute for runtime, nothing to bother really
    {
        print("move client");

        var randomPosition = GetRandomPostionOnPlanet();
        Position.Value = randomPosition;    //store the randomed 

        MovePostionClient(randomPosition);

        print(randomPosition);
    }

    void MovePostionClient(Vector3 newPos)
    {
        transform.position = newPos;    //keep the position at last randomed Vector3
    }

    static Vector3 GetRandomPostionOnPlanet()  //random position method and return as Vector3
    {
        return new Vector3(Random.Range(3f, -3f), 1f, Random.Range(3f, -3f));   //random the pos in x-z coordinate
    }

    //private void Update()   //Update, nothing special
    //{
        //transform.position = Position.Value;    //keep the position at last randomed Vector3
        //**tihs was once used in Update and moved to submit pos-change - request, but had been removed to make player controlling functional
    //}
}