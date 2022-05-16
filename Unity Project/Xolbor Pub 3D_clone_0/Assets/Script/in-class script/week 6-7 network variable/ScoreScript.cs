using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ScoreScript : NetworkBehaviour
{
    TMP_Text p1Text;
    TMP_Text p2Text;
    MainPlayer mainPlayer;
    public NetworkVariable<int> Score = new NetworkVariable<int>(NetworkVariableReadPermission.Everyone, 5);

    [ServerRpc]
    public void UpdateClientScoreServerRpc(int newScore)
    {
        Score.Value += newScore;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsLocalPlayer) return;

        if (collision.gameObject.tag == "DeathZone")
        {
            UpdateClientScoreServerRpc(-1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        p1Text = GameObject.Find("P1Score").GetComponent<TMP_Text>();
        p2Text = GameObject.Find("P2Score").GetComponent<TMP_Text>();
        mainPlayer = GetComponent<MainPlayer>();
    }

    private void UpdatePlayerNameAndScore()
    {
        if (IsOwnedByServer)
        {
            p1Text.text = $"{mainPlayer.PlayerName.Value} : {Score.Value}";
        }
        else
        {
            p2Text.text = $"{mainPlayer.PlayerName.Value} : {Score.Value}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerNameAndScore();
    }
}
