using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public bool gamePaused = false;
    public GameObject pauseCanvas;
    public ControllerManager controllerManager;

    public void PauseInput()
    {
        if (gamePaused) ResumeGame();
        else PauseGame();
    }

    public void ResumeGame()
    {
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    public void PauseGame()
    {
        pauseCanvas.SetActive(true);
        Time.timeScale = 0f;

        gamePaused = true;
    }
}
