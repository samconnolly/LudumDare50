using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public static class GameHelper
{
    public static GameManager gameManager;
    public static Guy player;
    public static readonly float gravityScale = 10;
    public static float musicVolume = 1;
    public static float soundVolume = 1;
    public static bool victorious = false;
    public static bool started = false;
    
    public static int mapSize = 16;

    public static Tilemap GroundTileMap {
        get {return gameManager.groundTileMap;}
    }
    public static Tilemap TopTileMap {
        get {return gameManager.topTileMap;}
    }
    public static ThornGrowth Thorns {
        get {return gameManager.topTileMap.GetComponent<ThornGrowth>();}
    }
    public static Tilemap MinimapTileMap {
        get {return gameManager.minimapTileMap;}
    }
    public static TileBase ThornTile {
        get {return gameManager.thornTile;}
    }

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

    public static void LoseGame()
    {
        gameManager.LoseGame();
    }


}
