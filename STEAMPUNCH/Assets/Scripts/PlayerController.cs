using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { Demo, Pause }

public class PlayerController : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    [SerializeField] public float throwingForceX = 500.0f;
    [SerializeField] public float throwingForceY = 500.0f;
    private bool isFacingRight = true;

    public bool IsFacingRight { get { return isFacingRight; } }

    [SerializeField] private float pickUpRadius = 0.1f;
    private List<Collider2D> nearbyColliders = new List<Collider2D>();
    private Enemy nearbyKnockedEnemy;
    private bool isHoldingEnemy = false;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    //animation
    [SerializeField]
    bool isStanding = false;
    Animator animator;

    private GameState _state; // Game State enum
    public TMP_Text pauseText;
    public Image pauseBackground;

    [SerializeField] GameObject sceneManager;


    private void Start()
    {
        rb.freezeRotation = true;
        // Causing errors. Could prevent by only checking for animator if not null (I tried implementing this in the most literal sense and it did not work)
        animator = GetComponent<Animator>();

        _state = GameState.Demo; // Currently, there are only two states, and the default state is the demo.

        pauseBackground.gameObject.SetActive(false);
        pauseText.gameObject.SetActive(false);
    }
    void Update()
    {
        // The "Gameplay" State. All physics happens here.
        if (_state == GameState.Demo)
        {
            // Allows player to pause by pressing ESC
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 0;
                _state = GameState.Pause;
                pauseBackground.gameObject.SetActive(true);
                pauseText.gameObject.SetActive(true);
            }

            horizontal = Input.GetAxisRaw("Horizontal");
            if (horizontal != 0 && isStanding)
            {
                animator.SetFloat("xvelocity", 1);
            }
            if (horizontal == 0 && animator.GetFloat("xvelocity") == 1)
            {
                animator.SetFloat("xvelocity", 0);
            }

            if (Input.GetKey(KeyCode.Space) && IsGrounded())
            {
                rb.velocity += new Vector2(rb.velocity.x, jumpingPower) * Time.deltaTime;
                isStanding = false;
            }

            if (Input.GetKey(KeyCode.Space) && rb.velocity.y > 0f)
            {
                rb.velocity += new Vector2(rb.velocity.x, rb.velocity.y * 0.5f) * Time.deltaTime;
                isStanding = false;
            }

            if (Input.GetKeyDown(KeyCode.F) && !isHoldingEnemy && NearKnockedEnemy())
            {
                isHoldingEnemy = true;
                nearbyKnockedEnemy.GrabbedByPlayer(this);
            }
            else if (Input.GetKeyDown(KeyCode.F) && isHoldingEnemy)
            {
                isHoldingEnemy = false;
                nearbyKnockedEnemy.DroppedByPlayer();
                nearbyKnockedEnemy = null;
            }

            if (Input.GetKeyDown(KeyCode.Q) && isHoldingEnemy)
            {
                //Drop the enemy
                isHoldingEnemy = false;
                nearbyKnockedEnemy.ThrownByPlayer();
                nearbyKnockedEnemy = null;
            }

            if (Input.GetKeyDown(KeyCode.R) || transform.position.y < -30)
            {
                //transform.position = Vector3.zero;

                sceneManager.GetComponent<SceneManager>().ResetScene();
            }

            if (!IsGrounded())
            {
                animator.SetBool("IsJumping", true);
            }
            Flip();
        }
        // The Pause state. No updates or player control of any kind.
        else if (_state == GameState.Pause)
        {
            // Allows player to resume by pressing ESC
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 1;
                _state = GameState.Demo;
                pauseBackground.gameObject.SetActive(false);
                pauseText.gameObject.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        // jump/fall animation
        if (!isStanding)
        {
            animator.SetBool("IsJumping", true);
            animator.SetFloat("yvelocity", rb.velocity.y);
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }

    }

    private bool NearKnockedEnemy()
    {
        int knockedEnemyColliders = Physics2D.OverlapCircle(transform.position, pickUpRadius, new ContactFilter2D().NoFilter(), nearbyColliders);
        for (int i = knockedEnemyColliders - 1; i >= 0; i--)
        {
            Enemy temp;
            if (nearbyColliders[i].gameObject.TryGetComponent<Enemy>(out temp))
            {
                if (temp.CurrentState == EnemyStates.Knocked)
                {
                    nearbyKnockedEnemy = temp;
                    nearbyColliders.Clear();
                    return true;
                }
            }
        }
        nearbyColliders.Clear();
        return false;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isStanding = true;
    }

    private void Flip()
    {
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}