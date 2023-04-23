using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public Canvas pauseMenu;
    public Canvas settingsMenu;
    public Canvas controlsMenu;

    public Slider topSetting;
    public Button settingButton;
    public Button goBack;
    public Button controlButton;

    public GameObject gameUI;
    public static bool pausePressed;

    private void Update()
    {
        if(pausePressed)
        {
            pauseMenu.gameObject.SetActive(true);
            gameUI.SetActive(false);
            Time.timeScale = 0;
            pausePressed = false;
        }

        if(settingsMenu.gameObject.active || controlsMenu.gameObject.active)
        {
            pauseMenu.gameObject.SetActive(false);
        }

    }

    public void Resume()
    {
        Time.timeScale = 1;
        pauseMenu.gameObject.SetActive(false);
        pausePressed = false;
    }

    public void OpenSettings()
    {
        settingsMenu.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
        gameUI.SetActive(false);
        EventSystem.current.SetSelectedGameObject(topSetting.gameObject);
    }

    public void CloseSettings()
    {
        settingsMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(true);
        gameUI.SetActive(false);
        EventSystem.current.SetSelectedGameObject(settingButton.gameObject);
    }

    public void OpenControls()
    {
        controlsMenu.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(goBack.gameObject);
        gameUI.SetActive(false);
    }

    public void CloseControls()
    {
        controlsMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(true);
        gameUI.SetActive(false);
        EventSystem.current.SetSelectedGameObject(controlButton.gameObject);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
