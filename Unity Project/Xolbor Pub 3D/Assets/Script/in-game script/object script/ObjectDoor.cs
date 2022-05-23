using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDoor : MonoBehaviour
{
    public Transform teleportPosition;  //null object for teleport point
    public Transform blackPanel;            //black panel for teleport transition
    public Transform playerTransform;

    public void TeleportCall(Transform player)
    {
        playerTransform = player;
        Transform blackPanelObject = Instantiate(blackPanel);
        blackPanelObject.SetParent(GameObject.FindWithTag("MainCanvas").transform);

        Invoke("TeleportCommit", 1f);
    }
    private void TeleportCommit()
    {
        playerTransform.position = new Vector3(teleportPosition.position.x,
            playerTransform.position.y, teleportPosition.position.z);

        CancelInvoke("TeleportCommit");
    }
}