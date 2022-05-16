using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class test_control : NetworkBehaviour
{
    public GameObject currentSceneText;
    private bool canSwitchScene = true;
    LoginManager loginManager;

    private void Start()
    {
        if (FindObjectOfType<LoginManager>() != null)
        {
            loginManager = FindObjectOfType<LoginManager>();
        }
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) && Input.GetKey(KeyCode.Z))
        {
            SceneManager.LoadScene(0);
        }
        if ((Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) && Input.GetKey(KeyCode.Z))
        {
            SceneManager.LoadScene(1);
        }
        if ((Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) && Input.GetKey(KeyCode.Z))
        {
            SceneManager.LoadScene(2);
        }

        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.Z))
        {
            loginManager.Leave();
        }
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.Z) && canSwitchScene == true)
        {
            canSwitchScene = false;
            loginManager.Leave();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
