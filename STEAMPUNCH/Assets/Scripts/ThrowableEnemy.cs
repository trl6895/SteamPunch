using UnityEngine;

public class ThrowableEnemy : MonoBehaviour
{
    // The base enemy script found on any enemy
    Enemy baseEnemy;

    // Components that should be found in any enemy prefab
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private BoxCollider2D boxCollider;

    private PlayerController player;

    public Enemy BaseEnemy
    {
        get
        {
            return baseEnemy;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        baseEnemy = gameObject.GetComponent<Enemy>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (baseEnemy.CurrentState == EnemyStates.Grabbed)
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
        rb.velocity -= baseEnemy.Rb.velocity;

        // Update the enemy's state
        baseEnemy.CurrentState = EnemyStates.Grabbed;

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
        baseEnemy.CurrentState = EnemyStates.Knocked;

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
        baseEnemy.CurrentState = EnemyStates.Knocked;

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
