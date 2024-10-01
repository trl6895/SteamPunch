using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    bool isStanding = false;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        //walking animation
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

        if (!IsGrounded())
        {
            animator.SetBool("IsJumping", true);
        }
        Flip();
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