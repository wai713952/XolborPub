using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class test_scene_switch : MonoBehaviour
{
    //this script is for switching between the scene of online testing
    //there's no need to build seperate game for test in each scene
    //it's attached to scene button and text current scene

    public string sceneName;

    public void SwitchingScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == sceneName) { return; }
        SceneManager.LoadScene(sceneName);
    }
}
