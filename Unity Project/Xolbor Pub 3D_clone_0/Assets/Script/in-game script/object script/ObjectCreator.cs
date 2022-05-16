using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ObjectCreator : NetworkBehaviour
{
    public GameObject itemPrefab;

    public void CreateItem()
    {
        CreateItemServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void CreateItemServerRpc()
    {
        GameObject tempItemPrefab = Instantiate(itemPrefab, transform.position + new Vector3(0, 0, 2), transform.rotation);
        tempItemPrefab.GetComponent<NetworkObject>().Spawn();
    }
}
