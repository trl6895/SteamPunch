using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The states of the player that determine if they can be controlled or not
/// </summary>
public enum PlayerState { Free, Locked }

/// <summary>
/// The current fist that the player is punching with
/// </summary>
public enum CurrentFist { Right, Left }

public class PlayerController : MonoBehaviour
{
    // Fields ===========================================================================

    // References -------------------------------------------------------------
    [SerializeField] SceneManager sceneManager;

    // Movement ---------------------------------------------------------------
    private float horizontal;
    private float speed = 8f;
    [SerializeField] public float jumpingPower = 16f;
    [SerializeField] public float throwingForceX = 500.0f;
    [SerializeField] public float throwingForceY = 500.0f;
    private bool isFacingRight = true;

    // Interaction ------------------------------------------------------------
    [SerializeField] private float pickUpRadius = 0.1f;
    private List<Collider2D> nearbyColliders = new List<Collider2D>();
    private Enemy nearbyKnockedEnemy;
    public bool isHoldingEnemy = false;
    public float punchCooldownTimer = 0.0f;
    [SerializeField] public const float punchCooldown = 1.0f;
    [SerializeField] public CapsuleCollider2D punchCollider;

    // Collision --------------------------------------------------------------
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    // Animation --------------------------------------------------------------
    [SerializeField] bool isStanding = false;
    Animator animator;

    // Properties =======================================================================

    /// <summary>
    /// Gets whether the player is facing right or not
    /// </summary>
    public bool IsFacingRight 
    {
        get
        {
            return isFacingRight;
        }
    }

    // Methods ==========================================================================

    // Start is called before the first frame update
    private void Start()
    {
        // Stop the player from rotating
        rb.freezeRotation = true;

        // Instantiate the animator
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the gameplay is actively running:
        if (sceneManager.gameState == GameState.Demo)
        {
            // Animate the player
            Animate();
        }
        // If the game is paused:
        else if (sceneManager.gameState == GameState.Pause)
        {
            //Nothing here right now :(
        }
    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        // Update the player's velocity
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isStanding = true;
    }

    /// <summary>
    /// Detects whether the player is near a knocked enemy or not
    /// </summary>
    /// <returns>Whether the player is near a knocked enemy or not</returns>
    public bool NearKnockedEnemy()
    {
        // Get the amount of colliders that there are near the player
        int knockedEnemyColliders = Physics2D.OverlapCircle(transform.position, pickUpRadius, new ContactFilter2D().NoFilter(), nearbyColliders);

        // For each collider within picking-up radius:
        for (int i = knockedEnemyColliders - 1; i >= 0; i--)
        {
            // Create a temporary enemy object
            Enemy temp;

            // If the current nearby collider belongs to an enemy: (I think? I don't know what the out part means)
            if (nearbyColliders[i].gameObject.TryGetComponent<Enemy>(out temp))
            {
                // If the current enemy is knocked:
                if (temp.CurrentState == EnemyStates.Knocked)
                {
                    // Store a reference to the enemy in temp
                    nearbyKnockedEnemy = temp;

                    // Forget about all other nearby colliders
                    nearbyColliders.Clear();

                    return true;
                }
            }
        }

        // Forget about all of the nearby colliders
        nearbyColliders.Clear();

        return false;
    }

    /// <summary>
    /// Detects if the player is on the ground
    /// </summary>
    /// <returns>Whether the player is on the ground or not</returns>
    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    /// <summary>
    /// Animates the player
    /// </summary>
    private void Animate()
    {
        // Flip the player in the correct direction
        Flip();

        // Give the player's velocities to the animator
        animator.SetFloat("xvelocity", horizontal);
        animator.SetFloat("yvelocity", rb.velocity.y);

        // If the player is standing on the ground:
        if(isStanding)
        {
            // Tell the animator that the player is no longer jumping
            animator.SetBool("IsJumping", false);
        }
    }

    /// <summary>
    /// Flips the player's sprite to the direction that they're moving in
    /// </summary>
    private void Flip()
    {
        // If the player is moving:
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            // Swap isFacingRight
            isFacingRight = !isFacingRight;

            // Flip the player's transform horizontally
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    /// <summary>
    /// Makes the player walk either left or right, depending on input
    /// </summary>
    public void Walk()
    {
        // Set the player's horizontal direction based on input
        horizontal = Input.GetAxisRaw("Horizontal");
    }

    /// <summary>
    /// Makes the player jump
    /// </summary>
    public void Jump()
    {
        isStanding = false;

        // Tell the animator that the player is jumping
        animator.SetBool("IsJumping", true);

        rb.velocity += new Vector2(rb.velocity.x, jumpingPower) * Time.deltaTime;
    }

    /// <summary>
    /// Prevents the player from falling too fast
    /// </summary>
    public void HoldingJump()
    {
        isStanding = false;
        rb.velocity += new Vector2(rb.velocity.x, rb.velocity.y * 0.5f) * Time.deltaTime;
    }

    /// <summary>
    /// Picks up the nearby knocked enemy
    /// </summary>
    public void PickUpEnemy()
    {
        isHoldingEnemy = true;
        nearbyKnockedEnemy.GrabbedByPlayer(this);
    }

    /// <summary>
    /// Drops the currently held enemy
    /// </summary>
    public void DropEnemy()
    {
        isHoldingEnemy = false;
        nearbyKnockedEnemy.DroppedByPlayer();
        nearbyKnockedEnemy = null;
    }

    /// <summary>
    /// Throws the currently held enemy
    /// </summary>
    public void ThrowEnemy()
    {
        isHoldingEnemy = false;
        nearbyKnockedEnemy.ThrownByPlayer();
        nearbyKnockedEnemy = null;
    }

    /// <summary>
    /// Makes the player peform a punch
    /// </summary>
    public void Punch()
    {
        // Play a punching sound effect


        //If the player is facing right:
        if (isFacingRight)
        {
            // Move the player a bit rightward
            rb.AddForce(new Vector2(750.0f, 0.0f));
        }
        //Otherwise:
        else
        {
            // Move the player a bit leftward
            rb.AddForce(new Vector2(-750.0f, 0.0f));
        }

        // Initialize a list to hold all colliders that collide with the punch
        List<Collider2D> contacts = new List<Collider2D>();
        
        // Fill the list with all contacts
        punchCollider.OverlapCollider(new ContactFilter2D().NoFilter(), contacts);

        // For each contact point in the punch collider:
        for (int i = 0; i < contacts.Count; i++)
        {
            // Create a temporary enemy object
            Enemy temp;

            // If the current overlapping collider belongs to an enemy:
            if (contacts[i].gameObject.TryGetComponent<Enemy>(out temp))
            {
                // If the enemy is alive:
                if (temp.CurrentState == EnemyStates.Alive)
                {
                    // If the player is facing right:
                    if (isFacingRight)
                    {
                        // Punch the enemy
                        temp.Punched(new Vector2(300.0f, 200.0f));
                    }
                    // Otherwise:
                    else
                    {
                        // Punch the enemy
                        temp.Punched(new Vector2(-300.0f, 200.0f));
                    }
                }
            }
        }

    }
}