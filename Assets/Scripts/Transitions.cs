using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transitions : MonoBehaviour
{
    public void characterChange()
    {

    }

    public void startGame()
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
