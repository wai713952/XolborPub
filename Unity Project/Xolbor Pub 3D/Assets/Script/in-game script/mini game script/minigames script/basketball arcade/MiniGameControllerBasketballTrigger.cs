using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameControllerBasketballTrigger : MonoBehaviour
{
    //this script is tied to both 'moving target' and 'miss collider'.
    //triggerName will check if it's either one of them and will active different fucntion, hit or miss the target.

    public string triggerName = "target sign";
    MiniGameControllerBasketballScore miniGameControllerBasketballScore;
    Animator animator;

    private void Start()
    {
        miniGameControllerBasketballScore.GetComponent<MiniGameControllerBasketballScore>();
        if (GetComponent<Animator>() != null) { animator = GetComponent<Animator>(); }
    }

    private void OnTriggerEnter(Collider ball)
    {
        if (ball.tag == "basketball" && this.gameObject.tag == "target sign")
        {
            miniGameControllerBasketballScore.ballHit();
            Destroy(ball.gameObject);
        }

        if (ball.tag == "basketball" && this.gameObject.tag == "miss collider")
        {
            miniGameControllerBasketballScore.ballMiss();
            Destroy(ball.gameObject);
            animator.speed += 0.1f;
        }
    }
}
