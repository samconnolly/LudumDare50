using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameHelper
{
    public static GameManager gameManager;
    public static Guy player;
    public static readonly float gravityScale = 10;
    public static float musicVolume = 1;
    public static float soundVolume = 1;
    public static bool victorious = false;
    public static bool started = false;
    public static int crystals = 0;

    public static void QuitGame()
    {
        Application.Quit();
    }

    public static void LoadGameScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public static void LoadMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }
  
    public static void LoadTreeScene()
    {
        started = true;
        if (crystals >= 5)
        {
            victorious = true;
        }
        SceneManager.LoadScene("Menu");
    }

}
