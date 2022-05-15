using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game_scene_target : MonoBehaviour
{
    public game_scene_controller game_scene_controller;
    Animator animator;

    private void Start()
    {
        game_scene_controller.GetComponent<game_scene_controller>();
        animator = GetComponent<Animator>();
    }

    public void OnTriggerEnter(Collider ball)
    {
        if (ball.tag == "Ball")
        {
            Destroy(ball.gameObject);
            game_scene_controller.HitTarget();
            animator.speed += 0.1f;
        }
    }
}
