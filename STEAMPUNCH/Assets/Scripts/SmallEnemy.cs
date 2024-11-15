using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallEnemy : Enemy
{
    //animation
    [SerializeField]
    Animator animator;

    private float walkDistance;
    [SerializeField] private float setWalkDistance;
    [SerializeField] private Transform playerTrackCheck;
    [SerializeField] private float followSpeed;
    private bool facingLeft;
    private bool foundPlayer = false;
    private Vector3 movement;
    EnemyStates state;


    // Start is called before the first frame update
    void Start()
    {
        state = EnemyStates.Alive;
        movement = new Vector3(0.0f, 0.0f, 0.0f);
        facingLeft = true;
        walkDistance = setWalkDistance;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(player.Health);
        switch (CurrentState)
        {
            case EnemyStates.Alive:
                if (foundPlayer) { TargetPlayer(); }
                else 
                {
                    Walk();
                    SearchPlayer();
                }
                DamagePlayer();
                break;
            case EnemyStates.Knocked:
                if (!animator.GetBool("IsKnocked"))
                {
                    animator.SetBool("IsKnocked", true);
                }
                break;
            case EnemyStates.Grabbed:
                break;
            default:
                break;
        }

    }

    public void Walk()
    {
        animator.SetFloat("Chase", 0);
        if (Physics2D.OverlapArea(new Vector2(floorCheck.position.x - (floorCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            floorCheck.position.y + (floorCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            new Vector2(floorCheck.position.x + (floorCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            floorCheck.position.y - (floorCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            groundLayer) && walkDistance >= 0.0f)
        {
            transform.position -= new Vector3(speed, 0.0f, 0.0f) * Time.deltaTime;
            walkDistance -= Mathf.Abs(speed) * Time.deltaTime;
        }
        else
        {
            transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
            speed *= -1;
            knockbackForce *= -1;
            walkDistance = setWalkDistance;
        }

    }

    public void SearchPlayer()
    {
        if (Physics2D.OverlapArea(new Vector2(playerTrackCheck.position.x - (playerTrackCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            playerTrackCheck.position.y + (playerTrackCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            new Vector2(playerTrackCheck.position.x + (playerTrackCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            playerTrackCheck.position.y - (playerTrackCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            playerLayer))
        {
            foundPlayer = true;
            float initialHeight = transform.position.y;
            Rb.AddForce(new Vector2(0.0f, 400.0f));
            while (transform.position.y != initialHeight)
            {
                Debug.Log("hello");
                continue;
            }
        }
    }

    public void TargetPlayer()
    {
        animator.SetFloat("Chase", 1);
        if (player.transform.position.x > transform.position.x)
        {
            transform.position += new Vector3(followSpeed, 0.0f, 0.0f) * Time.deltaTime;
            transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
            speed *= -1;
        }
        else if (player.transform.position.x < transform.position.x)
        {
            transform.position -= new Vector3(followSpeed, 0.0f, 0.0f) * Time.deltaTime;
            transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
            speed *= -1;
        }
    }

    //public void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireCube(new Vector2(collisionCheck.position.x - (collisionCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
    //        collisionCheck.position.y + (collisionCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
    //        new Vector2(collisionCheck.position.x + (collisionCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
    //        collisionCheck.position.y - (collisionCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)));
    //}



}
