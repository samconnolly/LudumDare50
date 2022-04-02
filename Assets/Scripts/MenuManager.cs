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
    private bool characterFree = false;
    public Vector2 edgeStartPosition;
    public Button startGameButton;
    public Button continueGameButton;
    public AudioSource musicPlayer;
    public AudioClip treeTheme;

    private float screenEdge = 14.5f;

    public Guy player;

    // Start is called before the first frame update
    void Start()
    {
        soundVolumeSlider.value = GameHelper.soundVolume;
        musicVolumeSlider.value = GameHelper.musicVolume;
        if (player != null)
        {
            GameHelper.player = player;
        }
        if (!inGame && GameHelper.victorious)
        {
            GoToVictoryMenu();
        }
        else if (!inGame && GameHelper.started)
        {
            inGame = true;
            player.transform.position = new Vector3(edgeStartPosition.x, edgeStartPosition.y, -1);
            UnfreezePlayer();
            HideMainMenu();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!inGame)
        {
            if (!characterFree)
            {
                if ((menuFading | introFading | introUnfading | waiting) & (Input.GetKeyDown(KeyCode.Escape) | Input.GetButtonDown("Fire2")))
                {
                    UnfreezePlayer();
                    waiting = false;
                    inGame = true;
                }
                else if ((menuFading | introFading | introUnfading | waiting) & (Input.GetButtonDown("Fire1") | Input.GetKeyDown(KeyCode.Return) | Input.GetKeyDown(KeyCode.Space)))
                {
                    if (introFrame < introText.Length - 1)
                    {
                        if (! introUnfading && ! menuFading)
                        {
                            introFrame += 1;                            
                        }
                        waitTime = interWait;
                        waiting = true;
                        introFading = false;
                        dialogueText.text = introText[introFrame];
                        introCanvas.GetComponent<CanvasGroup>().alpha = 1;
                        tooltipCanvas.GetComponent<CanvasGroup>().alpha = 1;
                    }
                    else if (introFrame == introText.Length - 1)
                    {
                        waiting = false;
                        introFading = true;
                        introCanvas.GetComponent<CanvasGroup>().alpha = 0;
                    }
                    introUnfading = false;
                    menuFading = false;
                }
                else if (!waiting && (Input.GetButtonDown("Fire1") | Input.GetKeyDown(KeyCode.Return) | Input.GetKeyDown(KeyCode.Space))
                         && ! Input.GetMouseButtonDown(0))
                {
                    StartGame();
                }

                if (menuFading)
                {
                    menuCanvas.GetComponent<CanvasGroup>().alpha -= fadeRate;
                    if (menuCanvas.GetComponent<CanvasGroup>().alpha <= 0)
                    {
                        menuFading = false;
                        waiting = true;
                        waitTime = interWait;
                        introFrame = 0;
                        dialogueText.text = introText[0];
                    }
                }
                if (introUnfading)
                {
                    introCanvas.GetComponent<CanvasGroup>().alpha += fadeRate;
                    tooltipCanvas.GetComponent<CanvasGroup>().alpha += fadeRate;
                    if (introCanvas.GetComponent<CanvasGroup>().alpha >= 1)
                    {
                        introUnfading = false;
                        waiting = true;
                        waitTime = readWait;
                    }
                }
                if (introFading)
                {
                    introCanvas.GetComponent<CanvasGroup>().alpha -= fadeRate;
                    if (introCanvas.GetComponent<CanvasGroup>().alpha <= 0)
                    {
                        introFading = false;
                        waiting = true;
                        waitTime = interWait;
                        introFrame += 1;
                        if (introFrame < introText.Length)
                        {
                            dialogueText.text = introText[introFrame];
                        }
                        else
                        {
                            UnfreezePlayer();
                            waiting = false;
                            inGame = true;
                        }
                    }
                }

                if (waiting)
                {
                    waitTimer += Time.deltaTime;
                    if (waitTimer >= waitTime)
                    {
                        waiting = false;
                        waitTimer = 0;
                        if (waitTime == interWait)
                        {
                            introUnfading = true;
                        }
                        else
                        {
                            introFading = true;
                        }
                    }

                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape) | Input.GetButtonDown("Fire2"))
                {
                    ShowMainMenu();
                }
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

    public void QuitGame()
    {
        GameHelper.QuitGame();
    }

    public void StartGame()
    {
        GameHelper.started = true;
        GameHelper.crystals = 0;
        menuFading = true;
        foreach (Button button in menuButtons)
        {
            button.enabled = false;
        }
        menuCanvas.gameObject.SetActive(false);
        optionsCanvas.gameObject.SetActive(false);
        introCanvas.gameObject.SetActive(true);
        tooltipCanvas.gameObject.SetActive(true);

        musicPlayer.clip = treeTheme;
        musicPlayer.Play();
    }

    public void ReturnToMenu()
    {
        GameHelper.LoadMenuScene();
    }

    public void GoToOptions()
    {
        menuCanvas.gameObject.SetActive(false);
        optionsCanvas.gameObject.SetActive(true);
        waiting = false;
    }

    public void GoToMainMenu()
    {
        waiting = false;
        inGame = false;
        menuFading = false;
        introUnfading = false;
        introFading = false;
        introCanvas.GetComponent<CanvasGroup>().alpha = 0;
        waitTimer = 0;
        introFrame = 0;
        player.Freeze();
        victoryCanvas.gameObject.SetActive(false);
        optionsCanvas.gameObject.SetActive(false);
        menuCanvas.gameObject.SetActive(true);       
    }

    public void GoToVictoryMenu()
    {
        inGame = false;
        GameHelper.started = false;
        player.Freeze();
        menuCanvas.gameObject.SetActive(false);
        victoryCanvas.gameObject.SetActive(true);
        GameHelper.victorious = false;
        waiting = true;
    }

    public void HideMainMenu()
    {
        menuCanvas.gameObject.SetActive(false);
        if (inGame)
        {
            player.Unfreeze();
        }
    }

    public void ShowMainMenu()
    {
        menuCanvas.gameObject.SetActive(true);
        menuCanvas.GetComponent<CanvasGroup>().alpha = 1;
        if (inGame)
        {
            player.Freeze();
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

    public void UnfreezePlayer()
    {
        player.Unfreeze();
        introCanvas.gameObject.SetActive(false);
        introUnfading = false;
        introFading = false;
        waiting = false;
        characterFree = true;
        tooltipCanvas.transform.GetChild(0).GetComponent<Text>().text = "[Use the arrow keys/left thumbstick to move, space/A to jump]";
    }
}

