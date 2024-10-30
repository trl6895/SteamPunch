using UnityEngine;

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
        // If on main menu screen:
        if (sceneManager.gameState == GameState.Main)
        {
            // Important! Right now, we have one "level" and it's a demo,
            // so I have this commented-out to effectively "skip over" visiting what would be the level select screen.

            /*

            // Press [ESC]
            // Closes the game
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            // Press ANY  OTHER KEY
            // Continues to level select screen
            else if (Input.anyKey)
            {
                sceneManager.LevelSelectMenu();
            }

            */
        }

        // If on level select screen:
        else if (sceneManager.gameState == GameState.LevelSelect)
        {
            // Press [ESC]
            // Returns to title screen
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                sceneManager.MainMenu();
            }
        }

        // If the gameplay is actively running:
        else if (sceneManager.gameState == GameState.Demo)
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
            if (Input.GetKeyDown(KeyCode.Space) && player.IsGrounded())
            {
                player.Jump();
            }

            if (Input.GetKeyDown(KeyCode.Space) && player.CurrentState == PlayerState.Locked)
            {
                player.Jump();
            }

            // Holding [SPACE] after jumping
            // Prevents the player from falling too fast
            //if (Input.GetKey(KeyCode.Space) && player.rb.velocity.y > 0f)
            //{
            //    player.HoldingJump();
            //}

            // Press [Mouse 1] while the player is not holding an enemy and while they are near a knocked enemy
            // Make the player pick up nearby knocked enemy
            if (Input.GetKeyDown(KeyCode.Mouse1) && !player.isHoldingEnemy && player.NearKnockedEnemy())
            {
                player.PickUpEnemy();
            }
            // Press [F] while the player is holding an enemy
            // Make the player drop the enemy
            else if (Input.GetKeyDown(KeyCode.Mouse1) && player.isHoldingEnemy)
            {
                player.DropEnemy();
            }

            // Press [Mouse 0] while the player
            else if (Input.GetKeyDown(KeyCode.Mouse0) && !player.isHoldingEnemy)
            {
                player.Punch();
            }
            // Press [Mouse 0] while the player is holding an enemy
            // Make the player throw the enemy
            else if (Input.GetKeyDown(KeyCode.Mouse0) && player.isHoldingEnemy)
            {
                //player.ThrowEnemy();
                player.SurfEnemy();
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