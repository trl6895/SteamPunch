using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    // Fields ===========================================================================

    // References -------------------------------------------------------------
    [SerializeField] public PlayerController player;
    [SerializeField] public SceneManager sceneManager;

    // Methods ==========================================================================

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If the gameplay is actively running:
        if (sceneManager.gameState == GameState.Demo)
        {
            // Press [ESC]
            // Pauses the game
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                sceneManager.Pause();
            }

            // Press/hold [A] or press/hold [D]
            // Makes the player walk left [A] or right [D]
            player.Walk();

            // Press [SPACE] while the player is grounded
            // Make the player jump
            if (Input.GetKey(KeyCode.Space) && player.IsGrounded())
            {
                player.Jump();
            }

            // Holding [SPACE] after jumping
            // Prevents the player from falling too fast
            if (Input.GetKey(KeyCode.Space) && player.rb.velocity.y > 0f)
            {
                player.HoldingJump();
            }

            // Press [F] while the player is not holding an enemy and while they are near a knocked enemy
            // Make the player pick up nearby knocked enemy
            if (Input.GetKeyDown(KeyCode.F) && !player.isHoldingEnemy && player.NearKnockedEnemy())
            {
                player.PickUpEnemy();
            }
            // Press [F] while the player is holding an enemy
            // Make the player drop the enemy
            else if (Input.GetKeyDown(KeyCode.F) && player.isHoldingEnemy)
            {
                player.DropEnemy();
            }

            // Press [Q]
            if (Input.GetKeyDown(KeyCode.Q) && !player.isHoldingEnemy)
            {
                player.Punch();
            }
            // Press [Q] while the player is holding an enemy
            // Make the player throw the enemy
            else if (Input.GetKeyDown(KeyCode.Q) && player.isHoldingEnemy)
            {
                player.ThrowEnemy();
            }

            // Press [R]
            // Reset the scene
            if (Input.GetKeyDown(KeyCode.R))
            {
                sceneManager.ResetScene();
            }
        }
        // If the game is paused:
        else if (sceneManager.gameState == GameState.Pause)
        {
            // Press [ESC]
            // Unpause the game
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                sceneManager.Unpause();
            }
        }
    }
}