using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerControllerTextChat : NetworkBehaviour
{
    public GameObject chatText;
    public GameObject chatText2;
    private TMP_InputField chatInputField;

    public GameObject whitePanelPrefab;
    public Transform whitePanelPosition;
    public GameObject whitePanelObject;

    public NetworkVariable<NetworkString> textNetwork = new NetworkVariable<NetworkString>();
    public bool canEnterChat;

    GameObject chatBox;
    MainPlayer mainPlayer;

    private void Start()
    {

        if (IsClient && IsOwner)
        {
            chatBox = GameObject.Find("chat input field");
            chatInputField = chatBox.GetComponent<TMP_InputField>();
            mainPlayer = GetComponent<MainPlayer>();
            canEnterChat = true;
            chatBox.SetActive(false);
        }
    }
    private void Update()
    {
        if (IsClient && IsOwner)
        {
            if (whitePanelObject != null)
            {
                whitePanelObject.transform.position = whitePanelPosition.transform.position;
                whitePanelObject.transform.rotation = whitePanelPosition.transform.rotation;
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                PlayerToggleChatField();
            }

            if (chatBox == null || chatBox.activeSelf == false)
            {
                return;
            }
            else
            {
                chatInputField.ActivateInputField();
                if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && canEnterChat == true)
                {
                    canEnterChat = false;
                    SendChatServerRpc(chatInputField.text);
                    StartCoroutine(WaitForChat());
                    chatInputField.text = "";
                }
            }
        }
    }
    private void PlayerToggleChatField()
    {
        mainPlayer.canControlCharacterMovement = !mainPlayer.canControlCharacterMovement;

        if (chatBox.activeSelf == false)
        {
            chatBox.SetActive(true);
            chatInputField.text = "";
        }
        else
        {
            chatBox.SetActive(false);
        }
    }
    [ServerRpc]
    private void SendChatServerRpc(string text)
    {
        textNetwork.Value = text;
        SendChatClientRpc(text);
    }
    [ClientRpc]
    private void SendChatClientRpc(string text)
    {
        chatText.GetComponent<TMP_Text>().text = text;
        chatText2.GetComponent<TMP_Text>().text = text;
        whitePanelObject = Instantiate(whitePanelPrefab, whitePanelPosition.transform.position, whitePanelPosition.transform.rotation);
    }

    IEnumerator WaitForChat()
    {
        yield return new WaitForSeconds(2f);
        ClearChatServerRpc();
        canEnterChat = true;
        StopAllCoroutines();
    }
    [ServerRpc]
    private void ClearChatServerRpc()
    {
        ClearChatClientRpc();
    }
    [ClientRpc]
    private void ClearChatClientRpc()
    {
        chatText.GetComponent<TMP_Text>().text = "";
        chatText2.GetComponent<TMP_Text>().text = "";
        Destroy(whitePanelObject);
    }
}
