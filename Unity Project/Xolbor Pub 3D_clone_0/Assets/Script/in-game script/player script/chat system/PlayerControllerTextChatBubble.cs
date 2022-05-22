using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerControllerTextChatBubble : NetworkBehaviour
{
    public GameObject chatBox;
    public TMP_InputField chatInputFIeld;
    public NetworkVariable<NetworkString> chatTextNetwork = new NetworkVariable<NetworkString>();

    public GameObject chatTextBubblePrefab;
    public Transform chatTextBubbleSpawnPoint;
    public GameObject chatTextTempObject;
    public List<GameObject> chatTextBubbleList = new List<GameObject>();

    public MainPlayer mainPlayer;

    private void Start()
    {
        if (IsClient && IsOwner)
        {
            chatBox = GameObject.Find("chat input field");
            chatInputFIeld = chatBox.GetComponent<TMP_InputField>();
            mainPlayer = GetComponent<MainPlayer>();
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
                chatInputFIeld.ActivateInputField();
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    if (chatTextBubbleList.Count > 0)
                    {
                        for (int i = 0; i < chatTextBubbleList.Count; i++)
                        {
                            if (chatTextBubbleList[chatTextBubbleList.Count - 1] == null)
                            {
                                chatTextBubbleList.Clear();
                            }

                            if (chatTextBubbleList.Count != 0 && chatTextBubbleList[i] == null)
                            {
                                chatTextBubbleList.Remove(chatTextBubbleList[i]);
                            }
                            if(chatTextBubbleList.Count != 0 && chatTextBubbleList[i] != null)
                            {
                                chatTextBubbleList[i].transform.position += Vector3.up * 0.5f;
                            }
                        }
                    }

                    CreateChatBubbleServerRpc(chatInputFIeld.text);

                    chatTextTempObject.transform.SetParent(this.transform);


                    chatInputFIeld.text = "";
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
            chatInputFIeld.text = "";
        }
        else
        {
            chatBox.SetActive(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateChatBubbleServerRpc(string text)
    {
        chatTextNetwork.Value = text;
        CreateChatBubbleClientRpc(text);
    }
    [ClientRpc]
    private void CreateChatBubbleClientRpc(NetworkString text)
    {
        chatTextTempObject = Instantiate(chatTextBubblePrefab, chatTextBubbleSpawnPoint.transform.position, chatTextBubbleSpawnPoint.transform.rotation);
        chatTextTempObject.GetComponent<TMP_Text>().text = text;
        Destroy(chatTextTempObject.gameObject, 5f);
    }
}