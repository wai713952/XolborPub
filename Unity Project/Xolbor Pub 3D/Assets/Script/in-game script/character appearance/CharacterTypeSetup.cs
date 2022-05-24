using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterTypeSetup : NetworkBehaviour
{
    public NetworkVariable<int> headTypeIndexNetwork = new NetworkVariable<int>();
    public NetworkVariable<int> bodyTypeIndexNetwork = new NetworkVariable<int>();

    public List<GameObject> headObjectList = new List<GameObject>();
    public List<GameObject> bodyObjectList = new List<GameObject>();

    CharacterTypeChooseMenu characterTypeChooseMenu;

    private void Start()
    {
        if (IsClient && IsOwner)
        {
            characterTypeChooseMenu = GameObject.FindWithTag("TypeController").GetComponent<CharacterTypeChooseMenu>();
            SetCustomizationToNetworkServerRpc(characterTypeChooseMenu.headTypeIndex, characterTypeChooseMenu.bodyTypeIndex);
        }
        Invoke("ApplyCustomizationToCharacter", 3f);
    }

    [ServerRpc]
    public void SetCustomizationToNetworkServerRpc(int headTypeIndex, int bodyTypeIndex)
    {
        headTypeIndexNetwork.Value = headTypeIndex;
        bodyTypeIndexNetwork.Value = bodyTypeIndex;
    }
    
    public void ApplyCustomizationToCharacter()
    {
        for (int i = 0; i < headObjectList.Count + 1; i++)
        {
            if (headTypeIndexNetwork.Value == 0)
            {
                break;
            }
            if (i == headTypeIndexNetwork.Value)
            {
                headObjectList[i-1].SetActive(true);
                break;
            }
        }

        for (int i = 0; i < bodyObjectList.Count; i++)
        {
            if (i == bodyTypeIndexNetwork.Value)
            {
                bodyObjectList[i].SetActive(true);
                break;
            }
        }
    }
}
