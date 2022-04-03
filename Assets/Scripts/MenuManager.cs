using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public Canvas menuCanvas;
    public Canvas introCanvas;
    public Canvas optionsCanvas;
    public Canvas victoryCanvas;
    public Canvas defeatCanvas;
    public Canvas tooltipCanvas;
    public Canvas uiCanvas;
    public Button[] menuButtons;
    public Slider musicVolumeSlider;
    public Slider soundVolumeSlider;
    private float fadeRate = 0.01f;
    private bool menuFading = false;
    private bool introUnfading = false;
    private bool introFading = false;
    private bool waiting = false;
    private float waitTimer = 0;
    private float waitTime;
    private float interWait = 0.2f;
    private float readWait = 1.25f;
    public string[] introText;
    private int introFrame = 0;
    public Text dialogueText;
    public bool inGame = false;
    public Vector2 edgeStartPosition;
    public Button startGameButton;
    public Button continueGameButton;
    public AudioSource musicPlayer;
    public AudioClip treeTheme;

    private float screenEdge = 14.5f;

    // Start is called before the first frame update
    void Start()
    {
        soundVolumeSlider.value = GameHelper.soundVolume;
        musicVolumeSlider.value = GameHelper.musicVolume;

        if (!inGame && GameHelper.victorious)
        {
            GoToVictoryMenu();
        }
        else if (!inGame && GameHelper.defeated)
        {
            GoToDefeatMenu();
        } else if (inGame) {
            GameHelper.gameRunning = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inGame) {
            if (!GameHelper.gameRunning)
            {
                if (Input.GetKeyDown(KeyCode.Escape) | Input.GetButtonDown("Fire2"))
                {
                    ShowMainMenu();
                }            
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape) | Input.GetButtonDown("Fire2"))
                {
                    if (menuCanvas.isActiveAndEnabled)
                    {
                        HideMainMenu();
                    }
                    else if (optionsCanvas.isActiveAndEnabled)
                    {
                        GoToMainMenu();
                    } else
                    {
                        ShowMainMenu();
                    }
                }
            }
        }

    }

    public void QuitGame()
    {
        GameHelper.QuitGame();
    }

    public void StartGame()
    {
        GameHelper.LoadGameScene();
    }

    public void ReturnToMenu()
    {
        GameHelper.LoadMenuScene();
    }

    public void GoToOptions()
    {
        menuCanvas.gameObject.SetActive(false);
        optionsCanvas.gameObject.SetActive(true);
    }

    public void GoToMainMenu()
    {
        GameHelper.gameRunning = false;
        victoryCanvas.gameObject.SetActive(false);
        defeatCanvas.gameObject.SetActive(false);
        optionsCanvas.gameObject.SetActive(false);
        menuCanvas.gameObject.SetActive(true);       
    }

    public void GoToVictoryMenu()
    {
        GameHelper.gameRunning = false;
        menuCanvas.gameObject.SetActive(false);
        victoryCanvas.gameObject.SetActive(true);
        GameHelper.victorious = false;
    }
    public void GoToDefeatMenu()
    {
        menuCanvas.gameObject.SetActive(false);
        defeatCanvas.gameObject.SetActive(true);
        GameHelper.defeated = false;
    }

    public void HideMainMenu()
    {
        menuCanvas.gameObject.SetActive(false);
        if (! GameHelper.gameRunning)
        {
            GameHelper.gameRunning = true;
        }
    }

    public void ShowMainMenu()
    {
        menuCanvas.gameObject.SetActive(true);
        menuCanvas.GetComponent<CanvasGroup>().alpha = 1;
        if (GameHelper.gameRunning)
        {
            GameHelper.gameRunning = false;
            startGameButton.gameObject.SetActive(false);
            continueGameButton.gameObject.SetActive(true);
        }
        else
        {
            continueGameButton.gameObject.SetActive(true);
            startGameButton.gameObject.SetActive(false);
        }
    }

    public void SetMusicVolume(float volume)
    {
        GameHelper.musicVolume = volume;
    }

    public void SetSoundVolume(float volume)
    {
        GameHelper.soundVolume = volume;
    }
}

