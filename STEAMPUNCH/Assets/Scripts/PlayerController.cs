using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Demo, Pause }

public class PlayerController : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    private bool isFacingRight = true;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    //animation
    bool isStanding = false;
    Animator animator;

    private GameState _state; // Game State enum

    private void Start()
    {
        animator = GetComponent<Animator>();
        _state = GameState.Demo; // Currently, there are only two states, and the default state is the demo.
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
            }
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
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
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}