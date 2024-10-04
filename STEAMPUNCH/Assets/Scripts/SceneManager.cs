using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// The states of the game
/// </summary>
public enum GameState { Demo, Pause }

public class SceneManager : MonoBehaviour
{
    // Fields ===========================================================================

    // References -------------------------------------------------------------
    [SerializeField] public TMP_Text pauseText;
    [SerializeField] public Image pauseBackground;

    // Gameplay management ----------------------------------------------------
    public GameState gameState;

    // Methods ==========================================================================

    // Start is called before the first frame update
    void Start()
    {
        // Set the game state to demo
        gameState = GameState.Demo;

        // Make sure the pause menu doesn't appear on startup
        pauseBackground.gameObject.SetActive(false);
        pauseText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Resets the scene
    /// </summary>
    public void ResetScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TempScene");
    }

    /// <summary>
    /// Pauses the game
    /// </summary>
    public void Pause()
    {
        //Update the gameState enum
        gameState = GameState.Pause;

        // Stop the passage of time
        Time.timeScale = 0;

        //Show the pause screen
        pauseBackground.gameObject.SetActive(true);
        pauseText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Unpauses the game
    /// </summary>
    public void Unpause()
    {
        //Update the gameState enum
        gameState = GameState.Demo;

        // Continue the passage of time at normal speed
        Time.timeScale = 1;

        //Hide the pause screen
        pauseBackground.gameObject.SetActive(false);
        pauseText.gameObject.SetActive(false);
    }
}