using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MenuInGame : MonoBehaviour
{
    [Header("Main Menu Contents")]
    public GameObject mainMenuGroup;
    public GameObject pageMainMenu;
    public GameObject pagePlayGame;
    public GameObject pageCustomAvatar;
    public GameObject windowHowToPlay;
    public GameObject pageSetting;
    public GameObject windowQuit;

    [Header("In-Game Menu Contents")]
    public GameObject InGameMenu;
    public GameObject pageInGameMenu;
    public GameObject pageSettingInGameMenu;
    public GameObject windowQuitInGame;

    [Header("Other Componnet")]
    public LoginManager login;
    public AudioMixer audioMixer;

    private void Start()
    {
        if (login.GetComponent<LoginManager>() != null)
        {
            login.GetComponent<LoginManager>();
        }

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && InGameMenu.activeSelf == false && login.isConnected == true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            InGameMenu.SetActive(true);
            pageInGameMenu.SetActive(true);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && InGameMenu.activeSelf == true && login.isConnected == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            InGameMenu.SetActive(false);
            pageInGameMenu.SetActive(false);
            pageSettingInGameMenu.SetActive(false);
            windowQuitInGame.SetActive(false);

        }
    }

    //---------- Main Menu Buttons ----------//

    public void PlayGameButton()
    {
        pageMainMenu.SetActive(false);
        pagePlayGame.SetActive(true);
    }
    public void PlayGameBackButton()
    {
        pageMainMenu.SetActive(true);
        pagePlayGame.SetActive(false);
    }

    public void CustomAvatarButton()
    {
        pageMainMenu.SetActive(false);
        pageCustomAvatar.SetActive(true);
    }
    public void CustomAvatarBackButton()
    {
        pageMainMenu.SetActive(true);
        pageCustomAvatar.SetActive(false);
    }

    public void HowToPlayButton()
    {
        pageMainMenu.SetActive(false);
        windowHowToPlay.SetActive(true);
    }
    public void HowToPlayBackButton()
    {
        pageMainMenu.SetActive(true);
        windowHowToPlay.SetActive(false);
    }

    public void SettingButton()
    {
        pageMainMenu.SetActive(false);
        pageSetting.SetActive(true);
    }
    public void SettingBackButton()
    {
        pageMainMenu.SetActive(true);
        pageSetting.SetActive(false);
    }

    public void QuitButton()
    {
        windowQuit.SetActive(true);
        pageMainMenu.SetActive(false);
    }
    public void QuitYesButton()
    {
        login.Leave();
    }
    public void QuitNoButton()
    {
        windowQuit.SetActive(false);
        pageMainMenu.SetActive(true);
    }

    //---------- In-Game Menu Buttons ----------//

    public void CloseInGameMenuButton()
    {
        InGameMenu.SetActive(false);
    }
    public void SettingInGameButton()
    {
        pageSettingInGameMenu.SetActive(true);
        pageInGameMenu.SetActive(false);
    }
    public void SettingInGameBackButton()
    {
        pageSettingInGameMenu.SetActive(false);
        pageInGameMenu.SetActive(true);
    }
    public void QuitInGameButton()
    {
        windowQuitInGame.SetActive(true);
        pageInGameMenu.SetActive(false);
    }
    public void QuitNoInGameButton()
    {
        windowQuitInGame.SetActive(false);
        pageInGameMenu.SetActive(true);
    }

    //---------- Sound Controller ----------//

    public void SetVolumeMaster(float volume)
    {
        audioMixer.SetFloat("Master", volume);
    }
    public void SetVolumeMusic(float volume)
    {
        audioMixer.SetFloat("Music", volume);
    }
    public void SetVolumeSfx(float volume)
    {
        audioMixer.SetFloat("Sfx", volume);
    }
}
