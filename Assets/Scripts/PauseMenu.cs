using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public Canvas pauseMenu;
    public Canvas settingsMenu;

    public Slider topSetting;
    public Button settingButton;

    public static bool pausePressed;

    private void Update()
    {
        if(pausePressed)
        {
            pauseMenu.gameObject.SetActive(true);
            Time.timeScale = 0;
            pausePressed = false;
        }

        if(settingsMenu.gameObject.active)
        {
            pauseMenu.gameObject.SetActive(false);
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        pauseMenu.gameObject.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsMenu.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(topSetting.gameObject);
    }

    public void CloseSettings()
    {
        settingsMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(settingButton.gameObject);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
