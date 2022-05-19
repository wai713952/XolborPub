using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class MainPlayer : NetworkBehaviour
{
    //the MainPlayer script is attached to player object
    //the player object use ClientNetworkTransform, just know that it's better than typical NetworkTransform
    //created in week4 - netcode introduction(project setup, RPC-method, lauching as host/client/server)

    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    public NetworkVariable<NetworkString> PlayerName = new NetworkVariable<NetworkString>();

    public float speed = 15.0f;
    public float rotationSpeed = 30f;
    Rigidbody rigidbody;

    public Text namePrefab;
    private Text nameLabel;

    private LoginManager loginManager;

    public bool canControlCharacterMovement = true;

    private void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        GameObject canvas = GameObject.FindWithTag("MainCanvas");                       //reference the canvas from tag
        nameLabel = Instantiate(namePrefab, Vector3.zero, Quaternion.identity) as Text; //create name lable for player
        nameLabel.transform.SetParent(canvas.transform);                                //set name child of canvas
        if (IsServer)
        {
            PlayerName.Value = $"Player {OwnerClientId}";   //networkString.value is getter
        }
        if (IsClient && IsOwner) 
        {
            loginManager = GameObject.FindObjectOfType<LoginManager>();     //finding the LogInManager script
            if (loginManager != null)                                       //if it does exist
            {
                UpdateClientNameServerRpc(loginManager.playerNameInputField.text);  //update the name for client player from login input field
            }
        }
    }

    [ServerRpc]
    public void UpdateClientNameServerRpc(string name)  //passing name from inputfield
    {
        PlayerName.Value = name;                        //client player name is the same as input
    }


    private void Update()
    {
        SetPlayerName();
    }
    void SetPlayerName()
    {
        Vector3 nameLabelPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 0.5f, 0));
        nameLabel.transform.position = nameLabelPos;
        if (!string.IsNullOrEmpty(PlayerName.Value))
        {
            nameLabel.text = PlayerName.Value;
        }
    }

    private void OnDestroy()
    {
        if (nameLabel != null)
        {
            Destroy(nameLabel.gameObject);
        } 
    }


    private void FixedUpdate()
    {
        if (IsClient && IsOwner && canControlCharacterMovement == true)
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

    public void Move()  //move the player to random pose
    {
        if (NetworkManager.Singleton.IsServer)      //are you the server? if yes, random postion and move you(server) to that place immediately
        {
            var randomPosition = GetRandomPositionOnPlane();    //random the postion
            transform.position = randomPosition;                //change postion
            Position.Value = randomPosition;                    //store the position in vector3
        }
        else
        {
            SubmitPositionRequestServerRpc();   //if you are the client, you need ask/request the server to move you to new position
        }

    }

    [ServerRpc] //requesting the server
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = GetRandomPositionOnPlane();    //keep the position at last randomed Vector3
    }

    static Vector3 GetRandomPositionOnPlane()   //random position method and return as Vector3
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));   //random the pos in x-z coordinate
    }

    //private void Update()   //Update, nothing special
    //{
        //transform.position = Position.Value;    //keep the position at last randomed Vector3
        //**tihs was once used in Update and moved to submit pos-change - request, but had been removed to make player controlling functional
    //}
}