using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class game_scene_controller : MonoBehaviour
{

    public TMP_Text hpText;
    public TMP_Text scoreText;

    public GameObject throwButton;
    public GameObject loseText;
    public GameObject UIBlocker;
    public GameObject hitText;
    public GameObject blackPanel;
    public GameObject target;

    public GameObject ballPrefab;
    public float ballForce;
    public Transform ballSpawnPoint;

    public float HP;
    public float score;

    public void Start()
    {
        hpText.GetComponent<TMP_Text>();
        scoreText.GetComponent<TMP_Text>();
        hpText.text = "";
        scoreText.text = "";
        UIBlocker.SetActive(true);
        loseText.SetActive(false);
        throwButton.SetActive(false);
        blackPanel.SetActive(true);
        target.SetActive(false);
    }

    public void ThrowBall()
    {
        GameObject ballTemp;
        ballTemp = Instantiate(ballPrefab, ballSpawnPoint.position, ballSpawnPoint.rotation);
        ballTemp.GetComponent<Rigidbody>().AddForce(Vector3.forward * ballForce);
        UIBlocker.SetActive(true);
        Invoke("DisableBlocker", 0.2f);
    }
    public void DisableBlocker()
    {
        UIBlocker.SetActive(false);
    }

    public void HitTarget()
    {
        print("hit");
        GameObject hitTextTemp;
        hitTextTemp = Instantiate(hitText);
        Destroy(hitTextTemp.gameObject, 0.8f);

        if (HP + 10 > 100)
        {
            HP = 100;
        }
        else
        {
            HP += 10;
        }
        score++;
        hpText.text = "HP: " + HP.ToString();
        scoreText.text = "Score: " + score.ToString();
    }

    public void OnTriggerEnter(Collider ball) //miss shooting
    {
        if (ball.tag == "Ball")
        {
            print("miss");
            Destroy(ball.gameObject);
            if (HP - 30 <= 0)
            {
                hpText.text = "HP: " + "0";
                scoreText.text = "Score: " + score.ToString();
                loseText.SetActive(true);
                UIBlocker.SetActive(true);
                Invoke("QuitButton", 3f);
                return;
            }
            HP -= 30;
        }
        hpText.text = "HP: " + HP.ToString();
        scoreText.text = "Score: " + score.ToString();
    }
    

    public void StartButton()
    {
        Destroy(EventSystem.current.currentSelectedGameObject.gameObject);
        hpText.text = "HP: " + HP.ToString();
        scoreText.text = "Score: " + score.ToString();
        UIBlocker.SetActive(false);
        throwButton.SetActive(true);
        blackPanel.SetActive(false);
        target.SetActive(true);
    }

    public void QuitButton()
    {
        SceneManager.LoadScene("menu scene");
    }
}
