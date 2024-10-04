using System;
using UnityEngine;

/// <summary>
/// The states of an enemy
/// </summary>
public enum EnemyStates
{
    Alive, Knocked, Grabbed
}

public class Enemy : MonoBehaviour
{
    // Fields ===========================================================================

    // This field is serialized atm for testing purposes
    [SerializeField] EnemyStates currentState;

    private SpriteRenderer sprite;
    private BoxCollider2D boxCollider;

    private PlayerController player;

    [SerializeField] private Rigidbody2D rb;

    // Properties =======================================================================

    // Right now my idea for this is that an enemy needs to be
    // "Knocked" for it to be grabbed, so the player script will have
    // to use this getter to determine if an enemy is "Knocked" when
    // attempting to pick it up
    /// <summary>
    /// Gets the current state of the enemy
    /// </summary>
    public EnemyStates CurrentState
    {
        get
        {
            return currentState;
        }
    }

    // Methods ==========================================================================

    // Start is called before the first frame update
    void Start()
    {
        // Get the enemy's components
        sprite = gameObject.GetComponent<SpriteRenderer>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the enemy is alive:
        if (currentState == EnemyStates.Alive)
        {
            sprite.color = Color.white;
            // do alive stuff
        }
        // making this an else if for the time being, 
        // in case we add more states
        // Otherwise, if the enemy is knocked:
        else if (currentState == EnemyStates.Knocked)
        {
            sprite.color = Color.red;
            // do "dead" stuff
        }
        // Otherwise, if the enemy is grabbed:
        else if (currentState == EnemyStates.Grabbed)
        {
            sprite.color = Color.blue;
            AttachToPlayer();
        }
    }

    /// <summary>
    /// Sets up the enemy after being grabbed by the player
    /// </summary>
    /// <param name="player">The player</param>
    public void GrabbedByPlayer(PlayerController player)
    {
        // Remove the enemy's velocity
        rb.velocity -= rb.velocity;

        // Update the enemy's state
        currentState = EnemyStates.Grabbed;

        // Disable enemy collision and physics
        boxCollider.enabled = false;
        rb.isKinematic = true;
        
        // Get a reference to the player
        this.player = player;
    }

    /// <summary>
    /// Sets up the enemy after being dropped by the player
    /// </summary>
    public void DroppedByPlayer()
    {
        // Update the enemy's state
        currentState = EnemyStates.Knocked;

        // Enable enemy collision and physics
        boxCollider.enabled = true;
        rb.isKinematic = false;

        // Remove the reference to the player
        this.player = null;
    }

    /// <summary>
    /// Sets up the enemy after being thrown by the player
    /// </summary>
    public void ThrownByPlayer()
    {
        // Update the enemy's state
        currentState = EnemyStates.Knocked;

        // Enable enemy collision and physics
        boxCollider.enabled = true;
        rb.isKinematic = false;

        //If the player is facing right:
        if (player.IsFacingRight)
        {
            // Add a force to the enemy that sends it to the right
            rb.AddForce(new Vector2(player.throwingForceX, player.throwingForceY));
        }
        //Otherwise:
        else
        {
            // Add a force to the enemy that sends it to the left
            rb.AddForce(new Vector2(-player.throwingForceX, player.throwingForceY));
        }

        // Remove the reference to the player
        this.player = null;
    }

    /// <summary>
    /// Attaches the enemy to the player
    /// </summary>
    private void AttachToPlayer()
    {
        // If the player is facing right:
        if (player.IsFacingRight)
        {
            // Put the enemy on the right side of the player
            transform.position = new Vector3(player.transform.position.x + 1f, player.transform.position.y + 1f);
        }
        // Otherwise:
        else
        {
            // Put the enemy on the left side of the player
            transform.position = new Vector3(player.transform.position.x - 1f, player.transform.position.y + 1f);
        }
    }
}
