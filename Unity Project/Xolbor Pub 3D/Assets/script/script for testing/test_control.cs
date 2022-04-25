using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class test_control : NetworkBehaviour
{
    private TMP_Text currenSceneText;
    private bool canSwitchScene = true;
    private void Start()
    {
        currenSceneText = GetComponent<TMP_Text>();
        currenSceneText.text = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) && Input.GetKey(KeyCode.Z) && canSwitchScene == true)
        {
            if (SceneManager.GetActiveScene().name == ("scene1_netcode_intro")) { return; }
            canSwitchScene = false;
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("scene1_netcode_intro");
        }
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.Z) && canSwitchScene == true)
        {
            canSwitchScene = false;
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
