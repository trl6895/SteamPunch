using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] protected SpriteRenderer sprite;
    private BoxCollider2D boxCollider;

    public BoxCollider2D BoxCollider { get { return boxCollider; } }

    [SerializeField] public Rigidbody2D rb;

    public Rigidbody2D Rb { get { return rb; } }

    [SerializeField]
    float hitFlashTimer = 0f;

    [SerializeField] private int health = 1;

    [SerializeField] protected Transform floorCheck;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected PlayerController player;
    [SerializeField] protected LayerMask playerLayer;

    [SerializeField] protected Transform hitboxCollisionCheck;

    [SerializeField] protected float speed;
    [SerializeField] protected float knockbackForce;

    // Audio ------------------------------------------------------------------
    [SerializeField] public AudioSource sfx_knocked;

    public int Health { get { return health; } }

    public float HitFlashTimer
    {
        get
        {
            return hitFlashTimer;
        }
        set
        {
            hitFlashTimer = value;
        }
    }

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
        if (hitFlashTimer <= 0)
        {
            hitFlashTimer = 0;
        }
        // If the enemy is alive:
        if (currentState == EnemyStates.Alive)
        {
            // do alive stuff
        }
        // making this an else if for the time being, 
        // in case we add more states
        // Otherwise, if the enemy is knocked:
        else if (currentState == EnemyStates.Knocked)
        {
            //sprite.color = Color.red;

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
        hitFlashTimer = 1f;
        health--;

        if (health <= 0)
        {
            // Update the enemy's state
            currentState = EnemyStates.Knocked;

            // Play the knocked sound effect
            sfx_knocked.Play();
        }

        // Add a force to the enemy that sends it to the right
        rb.AddForce(force);
    }

    public void DamagePlayer()
    {
        if (Physics2D.OverlapArea(new Vector2(hitboxCollisionCheck.position.x - (hitboxCollisionCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            hitboxCollisionCheck.position.y + (hitboxCollisionCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            new Vector2(hitboxCollisionCheck.position.x + (hitboxCollisionCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            hitboxCollisionCheck.position.y - (hitboxCollisionCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            playerLayer) && player.InvicibilityTimer <= 0f)
        {
            player.Health -= 1;
            player.InvicibilityTimer = 1f;
            //player.GetComponent<Rigidbody2D>().AddForce(new Vector2(2.0f * knockbackForce, 3.0f));
        }
    }



}
