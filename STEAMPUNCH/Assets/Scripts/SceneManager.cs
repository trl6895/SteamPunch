using System; // Don't delete me!
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The states of the game
/// </summary>
public enum GameState { Title, LevelSelect, Options, Demo, Dead, Pause } // Currently 'Options' is unused and 'Demo' should be renamed

public class SceneManager : MonoBehaviour
{
    // Fields ===========================================================================

    private int selectedStage;

    private GameObject[] levelUI;
    private GameObject[] deathUI;

    // References -------------------------------------------------------------
    [SerializeField] public TMP_Text titleTitle;
    [SerializeField] public Button titleLevels_b;

    private NewInputHandler newInputHandler;

    // Level Select
    [SerializeField] public Button levelStage_b1;
    [SerializeField] public Button levelPlay_b;
    [SerializeField] public TMP_Text levelName;
    [SerializeField] public TMP_Text levelBlurb;

    [SerializeField] public Button deathRetry_b;
    [SerializeField] public Button deathQuit_b;
    [SerializeField] public TMP_Text deathTitle;
    [SerializeField] public TMP_Text deathFlavor;

    [SerializeField] public TMP_Text pauseTitle;
    [SerializeField] public Image pauseBackground;
    [SerializeField] public Button pauseExit_b;

    [SerializeField] public Camera mainCamera;

    // Gameplay management ----------------------------------------------------
    public GameState gameState;

    // Methods ==========================================================================
    private void Awake()
    {
        newInputHandler = new NewInputHandler();
    }

    // Start is called before the first frame update
    void Start()
    {
        levelUI = GameObject.FindGameObjectsWithTag("levelScreen"); // Objects need to be ACTIVE to be found!
        deathUI = GameObject.FindGameObjectsWithTag("deathScreen"); // See above

        // This determines what state the game will be in when it starts (!)
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("Menus").isLoaded)
        { TitleScreen(); }
        // AFAIK this is the only place I can put this
        else
        {
            foreach (GameObject x in deathUI)
            { x.gameObject.SetActive(false); }
        }
    }

    // Update is called once per frame
    void Update()
    { }

    // Currently unused
    private void OnEnable()
    {

        newInputHandler.UI.Pause.performed += PauseOrUnpause;
        newInputHandler.UI.Pause.Enable();

        newInputHandler.UI.Reset.performed += OnResetButtonClick;
        newInputHandler.UI.Reset.Enable();


        Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        switch (scene.name)
        {
            case "Menus":
                //TitleScreen();
                break;
            case "Level 1":
                // PlayStage();
                // selectedStage = 1;
                // SwitchToGame();
                break;
        }
    }

    private void OnDisable()
    {
        newInputHandler.UI.Pause.Disable();
        newInputHandler.UI.Reset.Disable();
    }

    private void OnResetButtonClick(InputAction.CallbackContext context)
    {
        ResetScene();
    }

    /// <summary>
    /// Restarts the current scene
    /// </summary>
    public void ResetScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Loads the selected scene (level).
    /// </summary>
    public void SwitchToGame()
    {
        if (selectedStage == 1)      { UnityEngine.SceneManagement.SceneManager.LoadScene("Level 1"); }
        else if (selectedStage == 2) { /*UnityEngine.SceneManagement.SceneManager.LoadScene("Stage2");*/ }
        else if (selectedStage == 3) { /*UnityEngine.SceneManagement.SceneManager.LoadScene("Stage3");*/ }
        else { }
    }

    /// <summary>
    /// Loads the Menus scene.
    /// </summary>
    public void SwitchToTitle() { UnityEngine.SceneManagement.SceneManager.LoadScene("Menus"); }

    /// <summary>
    /// Draws TITLE SCREEN UI.
    /// </summary>
    public void TitleScreen()
    {
        gameState = GameState.Title;

        // Cleanup !
        if (Time.timeScale != 1) { Time.timeScale = 1; }
        selectedStage = 0;
        levelPlay_b.interactable = false; // The 'PUNCH' button
        levelName.gameObject.SetActive(false);
        levelBlurb.gameObject.SetActive(false);

        // UI changes
        try
        {
            foreach (GameObject x in levelUI)
            { x.gameObject.SetActive(false); }
        }
        catch (NullReferenceException) { Debug.Log("Suppressing Error"); } // Stops error

        titleTitle.gameObject.SetActive(true);
        titleLevels_b.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(titleLevels_b.gameObject); // Triston's Controller Extravaganza
    }

    /// <summary>
    /// Draws LEVEL SELECT SCREEN UI.
    /// </summary>
    public void LevelSelectMenu()
    {
        gameState = GameState.LevelSelect;

        // UI changes
        titleTitle.gameObject.SetActive(false);
        titleLevels_b.gameObject.SetActive(false);

        foreach (GameObject x in levelUI)
        { x.gameObject.SetActive(true); }

        EventSystem.current.SetSelectedGameObject(levelStage_b1.gameObject); // Triston's Controller Extravaganza
    }

    /// <summary>
    /// Gets run upon choosing a stage to play - it (calls the function that) loads that stage.
    /// </summary>
    public void PlayStage()
    {
        gameState = GameState.Demo;
        if (Time.timeScale != 1) { Time.timeScale = 1; } // In case player died

        // UI changes
        foreach (GameObject x in levelUI) // Coming from level select
        { x.gameObject.SetActive(false); }
        foreach (GameObject x in deathUI) // Coming from dead (retry)
        { x.gameObject.SetActive(false); }

        levelBlurb.gameObject.SetActive(false);
        levelName.gameObject.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null); // Triston's Controller Extravaganza

        SwitchToGame();
    }

    // Death Screen =====================================================================

    /// <summary>
    /// Effectively pauses the game, lets the player retry or quit to title.
    /// </summary>
    public void Death()
    {
        gameState = GameState.Dead;

        Time.timeScale = 0; // Stop time (pauses all game physics)

        // UI changes
        foreach (GameObject x in deathUI)
        { x.gameObject.SetActive(true); }

        EventSystem.current.SetSelectedGameObject(deathRetry_b.gameObject); // Triston's Controller Extravaganza

    }

    // Pause & Unpause ==================================================================

    private void PauseOrUnpause(InputAction.CallbackContext context)
    {
        if (gameState == GameState.Demo)
        {
            Pause();
        }
        else if (gameState == GameState.Pause)
        {
            Unpause();
        }
        else if (gameState == GameState.Title)
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Pauses the game and displays the pause screen.
    /// </summary>
    public void Pause()
    {
        gameState = GameState.Pause;

        Time.timeScale = 0; // Stop time (pauses all game physics)

        // UI changes
        pauseBackground.gameObject.SetActive(true);
        pauseTitle.gameObject.SetActive(true);
        pauseExit_b.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(pauseExit_b.gameObject); // Triston's Controller Extravaganza
    }

    /// <summary>
    /// Unpauses the game (and hides the pause screen).
    /// </summary>
    public void Unpause()
    {
        gameState = GameState.Demo;

        Time.timeScale = 1; // Resume time

        // UI changes
        pauseBackground.gameObject.SetActive(false);
        pauseTitle.gameObject.SetActive(false);
        pauseExit_b.gameObject.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null); // Triston's Controller Extravaganza
    }

    // Stage Buttons ====================================================================

    /// <summary>
    /// Internally changes the selected stage (to be called by buttons on Level Select).
    /// Buttons must follow a specific naming convention: ending in level #.
    /// </summary>
    public void SelectStage()
    {
        // Dynamic button to level system, supports theoretically infinite levels.
        // Takes numbers from end of the button's name to assign to selectedStage

        for (int l = EventSystem.current.currentSelectedGameObject.name.Length - 1; l >= 0; l--)
        {
            if (Char.IsNumber(EventSystem.current.currentSelectedGameObject.name[l]))
            {
                selectedStage = ((int)EventSystem.current.currentSelectedGameObject.name[l] - '0') *
                    (int)Math.Pow(10, EventSystem.current.currentSelectedGameObject.name.Length - 1 - l);
            }
        }

        // This WILL set the level names and blurbs to be specific descriptions, written and stored here.

        if (selectedStage == 1)
        {
            levelName.text = "I  -  CLOCKTOWER";
            //levelBlurb.text = "...";
        }
        else if (selectedStage == 2)
        {
            //levelName.text = "II  -  ...";
            //levelBlurb.text = "...";
        }
        else if (selectedStage == 2)
        {
            //levelName.text = "III  -  ...";
            //levelBlurb.text = "...";
        }

        levelName.gameObject.SetActive(true);
        levelBlurb.gameObject.SetActive(true);

        levelPlay_b.interactable = true;

        EventSystem.current.SetSelectedGameObject(levelPlay_b.gameObject);
    }
}