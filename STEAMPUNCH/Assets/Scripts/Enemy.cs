using System;
using UnityEngine;

public enum EnemyStates
{
    Alive,
    Knocked,
    Grabbed
}

public class Enemy : MonoBehaviour
{

    // This field is serialized atm for testing purposes
    [SerializeField]
    EnemyStates currentState;

    private SpriteRenderer sprite;
    private BoxCollider2D boxCollider;

    private PlayerController player;

    [SerializeField]
    private Rigidbody2D rb;

    // Right now my idea for this is that an enemy needs to be
    // "Knocked" for it to be grabbed, so the playee script will have
    // to use this getter to determine if an enemy is "Knocked" when
    // attempting to pick it up
    public EnemyStates CurrentState { get { return currentState; } }

    // Start is called before the first frame update
    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == EnemyStates.Alive)
        {
            sprite.color = Color.white;
            // do alive stuff
        }
        // making this an else if for the time being, 
        // in case we add more states
        else if (currentState == EnemyStates.Knocked)
        {
            sprite.color = Color.red;
            // do "dead" stuff
        }

        else if (currentState == EnemyStates.Grabbed)
        {
            sprite.color = Color.blue;
            AttachToPlayer();
        }
    }

    public void GrabbedByPlayer(PlayerController player)
    {
        rb.velocity -= rb.velocity;
        currentState = EnemyStates.Grabbed;
        boxCollider.enabled = false;
        rb.isKinematic = true;
        this.player = player;
    }

    public void DroppedByPlayer()
    {
        currentState = EnemyStates.Knocked;
        boxCollider.enabled = true;
        rb.isKinematic = false;
        this.player = null;
    }

    public void ThrownByPlayer()
    {
        currentState = EnemyStates.Knocked;
        boxCollider.enabled = true;
        rb.isKinematic = false;
        //If the player is facing right:
        if (player.IsFacingRight)
        {
            rb.AddForce(new Vector2(player.throwingForceX, player.throwingForceY));
        }
        //Otherwise:
        else
        {
            rb.AddForce(new Vector2(-player.throwingForceX, player.throwingForceY));
        }
        this.player = null;
    }

    private void AttachToPlayer()
    {
        if (player.IsFacingRight)
        {
            transform.position = new Vector3(player.transform.position.x + 1f, player.transform.position.y + 1f);
        }

        else
        {
            transform.position = new Vector3(player.transform.position.x - 1f, player.transform.position.y + 1f);
        }
    }
}
