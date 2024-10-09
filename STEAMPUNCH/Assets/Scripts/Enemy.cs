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

    public BoxCollider2D BoxCollider { get { return boxCollider; } }

    [SerializeField] private Rigidbody2D rb;

    public Rigidbody2D Rb { get { return rb; } }

    [SerializeField] private int health = 1;

    public int Health { get { return health; } }

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
        set
        {
            currentState = value;
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
            //sprite.color = Color.red;
            KnockedOpacityFlash();
            // do "dead" stuff
        }
        // Otherwise, if the enemy is grabbed:
        // MOVED TO THROWABLE ENEMY SCRIPT
    }

    /// <summary>
    /// Sets up the enemy after being punched by the player
    /// </summary>
    /// <param name="force">The knockback force that the enemy receives after being punched</param>
    public void Punched(Vector2 force)
    {
        health--;

        if (health <= 0)
            // Update the enemy's state
            currentState = EnemyStates.Knocked;

        // Add a force to the enemy that sends it to the right
        rb.AddForce(force);
    }

    /// <summary>
    /// Creates pulsing opacity effect on knocked enemies to indicate that they're knocked
    /// </summary>
    public void KnockedOpacityFlash()
    {
        sprite.color = new Color(1f, 1f, 1f, (0.25f * Mathf.Sin(Time.fixedTime / 0.1f)) + 0.75f);
    }

}
