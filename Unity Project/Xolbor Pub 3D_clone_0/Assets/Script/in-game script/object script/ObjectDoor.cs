using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDoor : MonoBehaviour
{
    private ObjectInteractable objectInteractable;

    public Transform teleportPosition;  //null object for teleport point
    public Image blackPanel;            //black panel for teleport transition

    private void Start()
    {
        objectInteractable = GetComponent<ObjectInteractable>();
    }

    public void Teleport(Transform playerTransform)
    {
        Transform tempPlayerTransform = playerTransform;
        tempPlayerTransform.position = new Vector3(teleportPosition.position.x,
            tempPlayerTransform.position.y, teleportPosition.position.z);
    }
}