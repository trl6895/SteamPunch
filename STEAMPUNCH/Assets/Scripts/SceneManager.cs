using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// The states of the game
/// </summary>
public enum GameState { Main, LevelSelect, Options, Demo, Pause }

public class SceneManager : MonoBehaviour
{
    // Fields ===========================================================================

    // References -------------------------------------------------------------
    [SerializeField] public TMP_Text mainTitleText;
    [SerializeField] public TMP_Text mainContinueText;
    [SerializeField] public Button bPlayDemo;

    [SerializeField] public TMP_Text levelTitleText;

    [SerializeField] public TMP_Text pauseTitleText;
    [SerializeField] public Image pauseBackground;
    [SerializeField] public Button bBackToMain;

    // Gameplay management ----------------------------------------------------
    public GameState gameState;

    // Methods ==========================================================================

    // Start is called before the first frame update
    void Start()
    {
        // This determines what state the game will be in when it starts (!)
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("Menus").isLoaded)
        { gameState = GameState.Main; }
    }

    // Update is called once per frame
    void Update()
    { }

    /// <summary>
    /// Resets the scene
    /// </summary>
    public void ResetScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TempScene");
    }

    /// <summary>
    /// The title screen, referred to by the word "main"
    /// </summary>
    public void MainMenu()
    {
        // Set the scene back to the Menus scene, and resume time in case this was run from the pause menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menus");
        Time.timeScale = 1;

        // Update the gameState enum
        gameState = GameState.Main;

        // Display own UI
        mainTitleText.gameObject.SetActive(true);

        // ==================== Temporary ! ====================
        bPlayDemo.gameObject.SetActive(true);
        //mainContinueText.gameObject.SetActive(true);
    }

    /// <summary>
    /// The Level Select screen.
    /// </summary>
    public void LevelSelectMenu()
    {
        // Update the gameState enum
        gameState = GameState.LevelSelect;

        // Hide MainMenu UI
        mainTitleText.gameObject.SetActive(false);
        mainContinueText.gameObject.SetActive(false);

        // Display own UI
        levelTitleText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Function to be run on PlayDemo button click.
    /// </summary>
    public void PlayDemo()
    {
        // Update the gameState enum
        gameState = GameState.Demo;

        // Hide LevelSelectMenu UI
        //levelTitleText.gameObject.SetActive(false);

        // Run the scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("TempScene");
    }

    /// <summary>
    /// Pauses the game
    /// </summary>
    public void Pause()
    {
        // Update the gameState enum
        gameState = GameState.Pause;

        // Stop the passage of time
        Time.timeScale = 0;

        // Display Pause UI
        pauseBackground.gameObject.SetActive(true);
        pauseTitleText.gameObject.SetActive(true);
        bBackToMain.gameObject.SetActive(true);
    }

    /// <summary>
    /// Unpauses the game
    /// </summary>
    public void Unpause()
    {
        // Update the gameState enum
        gameState = GameState.Demo;

        // Continue the passage of time at normal speed
        Time.timeScale = 1;

        // Hide Pause UI
        pauseBackground.gameObject.SetActive(false);
        pauseTitleText.gameObject.SetActive(false);
        bBackToMain.gameObject.SetActive(false);
    }
}