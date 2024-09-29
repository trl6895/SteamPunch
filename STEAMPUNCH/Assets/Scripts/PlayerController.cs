using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    private bool isFacingRight = true;

    public bool IsFacingRight { get { return isFacingRight; } }

    [SerializeField] private float pickUpRadius = 0.1f;
    private List<Collider2D> nearbyColliders = new List<Collider2D>();
    private Enemy nearbyKnockedEnemy;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        rb.freezeRotation = true;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.Space) && IsGrounded())
        {
            rb.velocity += new Vector2(rb.velocity.x, jumpingPower) * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Space) && rb.velocity.y > 0f)
        {
            rb.velocity += new Vector2(rb.velocity.x, rb.velocity.y * 0.5f) * Time.deltaTime;
        }

        if (NearKnockedEnemy() && Input.GetKey(KeyCode.F))
        {
            nearbyKnockedEnemy.GrabbedByPlayer(this);
        }

        Flip();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool NearKnockedEnemy()
    {
        int knockedEnemyColliders = Physics2D.OverlapCircle(transform.position, pickUpRadius, new ContactFilter2D().NoFilter(), nearbyColliders);
        for (int i = 0; i < knockedEnemyColliders; i++)
        {
            Enemy temp;
            if (nearbyColliders[i].gameObject.TryGetComponent<Enemy>(out temp))
            {
                if (temp.CurrentState == EnemyStates.Knocked)
                {
                    nearbyKnockedEnemy = temp;
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
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