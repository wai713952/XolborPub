using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class test_scene_name : NetworkBehaviour
{
    private TMP_Text currenSceneText;
    private bool canSwitchScene = true;
    private void Start()
    {
        currenSceneText = GetComponent<TMP_Text>();
        NetworkManager.Singleton.Shutdown();
        currenSceneText.text = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) && canSwitchScene == true)
        {
            if (SceneManager.GetActiveScene().name == ("1 - netcode intro")) { return; }
            SceneManager.LoadScene("1 - netcode intro");
            canSwitchScene = false;
        }
        if ((Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) && canSwitchScene == true)
        {
            if (SceneManager.GetActiveScene().name == ("2 - connection approval")) { return; }
            SceneManager.LoadScene("2 - connection approval");
            canSwitchScene = false;
        }
        if (Input.GetKeyDown(KeyCode.R) && canSwitchScene == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}