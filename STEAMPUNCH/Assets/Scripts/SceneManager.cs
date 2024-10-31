using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System; // Don't delete me!

/// <summary>
/// The states of the game
/// </summary>
public enum GameState { Title, LevelSelect, Options, Demo, Pause }

public class SceneManager : MonoBehaviour
{
    // Fields ===========================================================================

    private int selectedStage;
    private GameObject[] levelUI;

    // References -------------------------------------------------------------
    [SerializeField] public TMP_Text titleTitle;
    [SerializeField] public Button titleLevels_b;

    [SerializeField] public Button levelPlay_b;
    [SerializeField] public TMP_Text levelName;
    [SerializeField] public TMP_Text levelBlurb;

    [SerializeField] public TMP_Text pauseTitleText;
    [SerializeField] public Image pauseBackground;
    [SerializeField] public Button bBackToMain;

    // Gameplay management ----------------------------------------------------
    public GameState gameState;

    // Methods ==========================================================================

    // Start is called before the first frame update
    void Start()
    {
        levelUI = GameObject.FindGameObjectsWithTag("levelScreen"); // Objects need to be ACTIVE to be found!

        // This determines what state the game will be in when it starts (!)
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("Menus").isLoaded)
        { TitleScreen(); } // If statement not really necessary.
    }

    // Update is called once per frame
    void Update()
    { }

    // This will draw UI following a scene switch.
    // This is vital to ensuring the game can find all of the UI
    private void OnEnable()
    {
        Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        switch (scene.name)
        {
            case "Menus":
                TitleScreen();
                break;
            case "Sprint2Level":
                //PlayStage(); Do NOT uncomment this
                break;
        }
    }

    /// <summary>
    /// Restarts the current scene - Justin, is this how you have it?
    /// </summary>
    public void ResetScene() { UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().ToString()); }

    /// <summary>
    /// Loads the selected scene (level).
    /// </summary>
    public void SwitchToGame()
    {
        if (selectedStage == 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Sprint2Level");
        }
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
        levelPlay_b.interactable = false;
        levelName.gameObject.SetActive(false);
        levelBlurb.gameObject.SetActive(false);

        // UI changes
        try { foreach (GameObject x in levelUI)
        { x.gameObject.SetActive(false); } }
        catch (NullReferenceException) { Debug.Log("STFU??"); } // Stops error

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
    }

    /// <summary>
    /// Gets run upon choosing a stage to play - it loads that stage.
    /// </summary>
    public void PlayStage()
    {
        gameState = GameState.Demo;

        // UI changes
        foreach (GameObject x in levelUI)
        { x.gameObject.SetActive(false); }

        levelBlurb.gameObject.SetActive(false);
        levelName.gameObject.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null); // Triston's Controller Extravaganza

        SwitchToGame();
    }

    // Pause & Unpause ==================================================================

    /// <summary>
    /// Pauses the game and displays the pause screen.
    /// </summary>
    public void Pause()
    {
        gameState = GameState.Pause;

        Time.timeScale = 0; // Stop time (pauses all game physics)

        // UI changes
        pauseBackground.gameObject.SetActive(true);
        pauseTitleText.gameObject.SetActive(true);
        bBackToMain.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(bBackToMain.gameObject); // Triston's Controller Extravaganza
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
        pauseTitleText.gameObject.SetActive(false);
        bBackToMain.gameObject.SetActive(false);

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
                selectedStage += ((int)EventSystem.current.currentSelectedGameObject.name[l] - '0') *
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
    }
}