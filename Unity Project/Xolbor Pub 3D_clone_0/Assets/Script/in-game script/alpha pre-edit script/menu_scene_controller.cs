using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class menu_scene_controller : MonoBehaviour
{
    public void LaunchGameButton()
    {
        SceneManager.LoadScene("game scene");
    }
}
