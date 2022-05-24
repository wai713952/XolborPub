using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerControllerTextChat : NetworkBehaviour
{
    public GameObject chatText;
    //public GameObject chatText2;
    private TMP_InputField chatInputField;

    public GameObject whitePanelPrefab;
    public Transform whitePanelPosition;

    public GameObject whitePanelObject;

    public NetworkVariable<NetworkString> textNetwork = new NetworkVariable<NetworkString>();
    public bool canEnterChat;

    GameObject chatBox;
    MainPlayer mainPlayer;
    Player_Movement player_Movement;

    private void Start()
    {
        if (IsClient && IsOwner)
        {
            chatBox = GameObject.Find("chat input field");

            chatInputField = chatBox.GetComponent<TMP_InputField>();
            mainPlayer = GetComponent<MainPlayer>();
            player_Movement = GetComponent<Player_Movement>();

            canEnterChat = true;
            chatBox.SetActive(false);
        }
    }
    private void Update()
    {
        if (IsClient && IsOwner)
        {
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
                    PlayerToggleChatField();
                }
            }
        }
    }
    private void PlayerToggleChatField()
    {
        mainPlayer.canControlCharacterMovement = !mainPlayer.canControlCharacterMovement;
        player_Movement.canControlCharacterMovement = !player_Movement.canControlCharacterMovement;

        if (chatBox.activeSelf == false)
        {
            chatBox.SetActive(true);
            chatBox.GetComponent<Animation>().Play();
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
        whitePanelObject.SetActive(true);

        chatText.GetComponent<TMP_Text>().text = text;
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
        whitePanelObject.SetActive(false);
    }
}
