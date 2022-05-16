using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MiniGameControllerBasketballScore : MonoBehaviour
{
    //basketball is single player mini-game, only one player can play at the time.
    //other player can see movement of the ball and score that fetch everytime the ball has been shot on the screen.
    //the score and hp will remain in local until the time is over for two players.
    //if time's up, send the value to the server that contains each players' score as array and compare to find the winner.

    private int localPlayerScore;
    private int localPlayerHealth;
    private float localPlayerTime = 30;
    private bool localPlayerGameOver = false;

    //following text will be display on server script
    //public TMP_Text serverScoreText;
    //public TMP_Text serverTimeText; 

    public GameObject UIBlocker;
    public GameObject basketballPrefab;
    public Transform basketballSpawnPoint;
    public float basketballForce;

    public void ballGameSetup()
    {

    }
    public void ballGameOver()
    {

    }
    public void ballShootButton()
    {
        GameObject basketballTemp;
        basketballTemp = Instantiate(basketballPrefab, basketballSpawnPoint.position, basketballSpawnPoint.rotation);
        basketballTemp.GetComponent<Rigidbody>().AddForce(Vector3.forward * basketballForce);

        GameObject UIBlockerTemp = Instantiate(UIBlocker);
        GameObject.Destroy(UIBlockerTemp, 0.2f);
    }
    public void ballHit()
    {
        if (localPlayerGameOver == true) { return; }

        localPlayerScore++;
        localPlayerHealth += 10;
    }
    public void ballMiss()
    {
        if(localPlayerGameOver == true) { return; }

        localPlayerHealth -= 10;

        if (localPlayerHealth <= 0)
        {
            localPlayerHealth = 0;
            localPlayerGameOver = true;
        }
    }

    private void Update()
    {
        localPlayerTime -= Time.deltaTime;
        int localPlayerTimeInt = Mathf.FloorToInt(localPlayerTime);

        if (localPlayerTime < 0)
        {
            localPlayerTime = 0;
            localPlayerGameOver = true;
            return;
        }
    }

    private void ServerTimeSend()
    {
        //sending time to server
    }
    private void ServerDataSend(int scoreDisplay, int healthDisplay)
    {
        //send the score to the server
        //display latest score and best score to every client
    }
}
