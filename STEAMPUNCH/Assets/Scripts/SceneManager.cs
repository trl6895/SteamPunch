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

    private int selectedStage; // Tracks currently selected stage. Changes on button select in menu, and resets when menu is loaded.

    // Arrays of tagged game objects
    private GameObject[] titleUI; // Contains all of the title select UI elements
    private GameObject[] titleCreditsUI; // Contains all of the title credits UI elements
    private GameObject[] levelUI; // Contains all* of the level select UI elements (excludes those with serialization necessary)
    private GameObject[] pauseUI; // Contains all* of the pause screen UI elements
    private GameObject[] deathUI; // Contains all* of the death screen UI elements
    private GameObject[] gameUI;  // Contains all of the on-screen in-game UI elements

    // Note that 'greyBackground' is used on both the pause and death screens, so it is not tagged.

    // References -------------------------------------------------------------

    [SerializeField] public Camera mainCamera;
    private NewInputHandler newInputHandler;

    // Fields that change states (buttons/activity, text/text, etc) DO need to be serialized

    // Serialized for T.C.E.
    [SerializeField] public Button titleLevels_b;
    [SerializeField] public Button titleCreditsBack_b;
    [SerializeField] public Button levelStage_b1;
    [SerializeField] public Button deathRetry_b;
    [SerializeField] public Button pauseExit_b;

    // Serialized to be directly edited (for example - text that changes)
    [SerializeField] public Button levelPlay_b;
    [SerializeField] public TMP_Text levelName;
    [SerializeField] public TMP_Text levelBlurb;
   

    // Serialized because it is used in multiple places and thus cannot be efficiently managed through a tag
    [SerializeField] public Image titleArt_placeholder_1;
    [SerializeField] public Image greyBackground;

    // Serialized SFX to be played at a time other than on button press (for example - on game pause)
    [SerializeField] public AudioClip button_sfx_hit;
    [SerializeField] public AudioClip button_sfx_short;

    // Gameplay management ----------------------------------------------------

    public GameState gameState;

    // Default Methods ==================================================================

    private void Awake()
    {
        newInputHandler = new NewInputHandler();

        AudioSource audio = GetComponent<AudioSource>();
        //DontDestroyOnLoad(audio);
    }

    // Start is called before the first frame update
    void Start()
    {
        titleUI = GameObject.FindGameObjectsWithTag("titleScreen"); // Objects need to be ACTIVE to be found!
        titleCreditsUI = GameObject.FindGameObjectsWithTag("titleCreditsScreen"); // See above
        levelUI = GameObject.FindGameObjectsWithTag("levelScreen"); // See above
        pauseUI = GameObject.FindGameObjectsWithTag("pauseScreen"); // See above
        deathUI = GameObject.FindGameObjectsWithTag("deathScreen"); // See above
        gameUI  = GameObject.FindGameObjectsWithTag("gameUI");      // See above

        // This determines what state the game will be in when it starts (!) and hides all UI that was enabled to be added to an array
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("Menus").isLoaded)
        {
            ShowOrHideUI(true, titleUI); // Not necessary due to coincidence but good standard
            TitleScreen();
        }
        // AFAIK this is the only place I can put this
        else
        {
            // Good practice to include all arrays here
            ShowOrHideUI(false, titleCreditsUI);
            ShowOrHideUI(false, pauseUI);
            ShowOrHideUI(false, levelUI);
            ShowOrHideUI(false, deathUI);
            //ShowOrHideUI(false, gameUI);
        }
    }

    // Update is called once per frame
    void Update() { }

    private void OnEnable()
    {
        newInputHandler.UI.Pause.performed += PauseOrUnpause;
        newInputHandler.UI.Pause.Enable();

        newInputHandler.UI.Reset.performed += OnResetButtonClick;
        newInputHandler.UI.Reset.Enable();

        Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        // Unused
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

    // Triston's controller extravaganza contd. =========================================

    /// <summary>
    /// Formality*
    /// </summary>
    private void OnDisable() { newInputHandler.UI.Pause.Disable(); newInputHandler.UI.Reset.Disable(); }

    /// <summary>
    /// Restarts the current scene - supports controller.
    /// </summary>
    /// <param name="context">Button pressed</param>
    private void OnResetButtonClick(InputAction.CallbackContext context) { ResetScene(); }

    /// <summary>
    /// Restarts the current scene
    /// </summary>
    public void ResetScene() 
    { 
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name); 
        Time.timeScale = 1.0f;
    }

    // Nav ==============================================================================

    /// <summary>
    /// Loads the selected scene (level).
    /// </summary>
    public void SwitchToGame()
    {
        // A more futureproof way to code this obviously does exist
        if (selectedStage == 1) { UnityEngine.SceneManagement.SceneManager.LoadScene("Level 1 Supreme"); }
        else if (selectedStage == 2) { /*UnityEngine.SceneManagement.SceneManager.LoadScene("Stage2");*/ }
        else if (selectedStage == 3) { /*UnityEngine.SceneManagement.SceneManager.LoadScene("Stage3");*/ }
        else { }
    }

    /// <summary>
    /// Loads the Menus scene.
    /// </summary>
    public void SwitchToTitle() { UnityEngine.SceneManagement.SceneManager.LoadScene("Menus"); }

    // UI ===============================================================================

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
        levelName.gameObject.SetActive(false); // Level name
        levelBlurb.gameObject.SetActive(false); // Level info

        // UI changes
        try { ShowOrHideUI(false, levelUI); }
        catch (NullReferenceException) { Debug.Log("Suppressing Error"); } // Stops error
        ShowOrHideUI(false, titleCreditsUI);
        ShowOrHideUI(true, titleUI);

        EventSystem.current.SetSelectedGameObject(titleLevels_b.gameObject); // Triston's Controller Extravaganza
        //EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Draws the CREDITS, as shown from the title screen.
    /// </summary>
    public void TitleCredits()
    {
        gameState = GameState.LevelSelect;

        // UI changes
        ShowOrHideUI(true, titleCreditsUI);
        ShowOrHideUI(false, titleUI);
        titleArt_placeholder_1.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(titleCreditsBack_b.gameObject); // Triston's Controller Extravaganza
    }

    /// <summary>
    /// Draws LEVEL SELECT SCREEN UI.
    /// </summary>
    public void LevelSelectMenu()
    {
        gameState = GameState.LevelSelect;

        // UI changes
        ShowOrHideUI(false, titleUI);
        ShowOrHideUI(true, levelUI);

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
        ShowOrHideUI(false, levelUI); // Coming from level select
        ShowOrHideUI(false, deathUI); // Coming from dead (retry)
        ShowOrHideUI(true, gameUI);

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
        ShowOrHideUI(true, deathUI);
        ShowOrHideUI(false, gameUI);
        greyBackground.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(deathRetry_b.gameObject); // Triston's Controller Extravaganza
    }

    // Pause & Unpause ==================================================================

    /// <summary>
    /// Formality*
    /// </summary>
    /// <param name="context"></param>
    private void PauseOrUnpause(InputAction.CallbackContext context)
    {
        if      (gameState == GameState.Demo)  { Pause(); }
        else if (gameState == GameState.Pause) { Unpause(); }
        else if (gameState == GameState.LevelSelect){ TitleScreen();}
        else if (gameState == GameState.Title) { Application.Quit(); }
    }

    /// <summary>
    /// Pauses the game and displays the pause screen.
    /// </summary>
    public void Pause()
    {
        gameState = GameState.Pause;
        Time.timeScale = 0; // Stop time (pauses all game physics)
        PlaySFX(button_sfx_hit);

        // UI changes
        ShowOrHideUI(true, pauseUI);
        ShowOrHideUI(false, gameUI);
        greyBackground.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(pauseExit_b.gameObject); // Triston's Controller Extravaganza
    }

    /// <summary>
    /// Unpauses the game (and hides the pause screen).
    /// </summary>
    public void Unpause()
    {
        gameState = GameState.Demo;
        Time.timeScale = 1; // Resume time
        PlaySFX(button_sfx_short);

        // UI changes
        ShowOrHideUI(false, pauseUI);
        ShowOrHideUI(true, gameUI);
        greyBackground.gameObject.SetActive(false);

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

        levelPlay_b.interactable = true; // Enables player to click 'PUNCH' button

        EventSystem.current.SetSelectedGameObject(levelPlay_b.gameObject);
    }

    // Helper functions =================================================================

    /// <summary>
    /// Helper function: Sets active or inactive all UI in an array - for use with arrays of tagged elements defined at the start of SceneManager.
    /// </summary>
    /// <param name="show">Bool - whether to show or hide the UI</param>
    /// <param name="UI">Array of GameObjects</param>
    public void ShowOrHideUI(bool show, GameObject[] UI)
    {
        foreach (GameObject x in UI)
        { x.gameObject.SetActive(show); }
    }

    /// <summary>
    /// Plays a sound effect
    /// </summary>
    /// <param name="audioClip">Desired sfx</param>
    public void PlaySFX(AudioClip audioClip)
    {
        GetComponent<AudioSource>().clip = audioClip;
        GetComponent<AudioSource>().Play();
    }
}