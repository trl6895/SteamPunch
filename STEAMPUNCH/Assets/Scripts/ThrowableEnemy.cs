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
        if (baseEnemy.CurrentState == EnemyStates.Knocked)
        {
            KnockedOpacityFlash();
        }
        else if (baseEnemy.CurrentState == EnemyStates.Grabbed)
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

        // Ensure that the enemy is in the correct position
        transform.position = player.holdingPosition;

        // Get the force that the enemy will be thrown with
        Vector2 force = new Vector2
            (
                Mathf.Cos(player.throwingAngle) * player.throwingForce,
                Mathf.Sin(player.throwingAngle) * player.throwingForce
            );

        // If the player is facing left:
        if (!player.IsFacingRight)
        {
            // Invert the force
            force *= -1.0f;
        }

        // Apply the force to the enemy's rigid body
        rb.AddForce(force);

        // Remove the reference to the player
        this.player = null;
    }


    /// <summary>
    /// Attaches the enemy to the player
    /// </summary>
    private void AttachToPlayer()
    {
        // If the player is facing right:
        //if (player.IsFacingRight)
        //{
        //    // Put the enemy on the right side of the player
        //    transform.position = new Vector3(player.transform.position.x + 1f, player.transform.position.y + 1f);
        //}
        //// Otherwise:
        //else
        //{
        //    // Put the enemy on the left side of the player
        //    transform.position = new Vector3(player.transform.position.x - 1f, player.transform.position.y + 1f);
        //}

        transform.position = player.holdingPosition;
    }

    /// <summary>
    /// Creates pulsing opacity effect on knocked enemies to indicate that they're knocked
    /// </summary>
    public void KnockedOpacityFlash()
    {
        sprite.color = new Color(1f, 1f, 1f, (0.25f * Mathf.Sin(Time.fixedTime / 0.1f)) + 0.75f);
    }
}
