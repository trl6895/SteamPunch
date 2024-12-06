using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;

public class MedEnemy : Enemy
{
    //animation
    //[SerializeField]
    //Animator animator;

    [SerializeField] private float avoidDistance;
    [SerializeField] private float avoidRange;
    [SerializeField] private float turnAroundTime;
    [SerializeField] private Transform playerTrackCheck;
    [SerializeField] protected Transform playerPos;
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected float attackFrequency;
    private Animator animator;
    protected float currentAttackCooldown;
    protected bool inCooldown;
    private bool facingLeft;
    private bool playerInRange = false;
    private Vector3 movement;
    EnemyStates state;


    // Start is called before the first frame update
    void Start()
    {
        state = EnemyStates.Alive;
        movement = new Vector3(0.0f, 0.0f, 0.0f);
        facingLeft = true;
        currentAttackCooldown = attackFrequency;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HitFlashTimer -= Time.deltaTime;
        if (HitFlashTimer <= 0)
        {
            HitFlashTimer = 0;
        }
        sprite.color = new Color(1, 1 - (HitFlashTimer / 3), 1 - (HitFlashTimer / 3), 1 - (HitFlashTimer / 3));


        //Debug.Log(player.Health);
        switch (CurrentState)
        {
            case EnemyStates.Alive:
                AvoidPlayer();
                TargetPlayer();
                DamagePlayer();
                break;
            case EnemyStates.Knocked:
                //if (!animator.GetBool("IsKnocked"))
                //{
                //    animator.SetBool("IsKnocked", true);
                //}
                animator.SetBool("isDead", true);
                break;
            case EnemyStates.Grabbed:
                break;
            default:
                break;
        }

    }

    public void AvoidPlayer()
    {
        if (Vector3.Distance(new Vector3(transform.position.x, 0.0f, 0.0f), new Vector3(playerPos.transform.position.x, 0.0f, 0.0f)) < avoidRange 
            && avoidDistance > 0.0f 
            && Physics2D.OverlapArea(new Vector2(floorCheck.position.x - (floorCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            floorCheck.position.y + (floorCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            new Vector2(floorCheck.position.x + (floorCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            floorCheck.position.y - (floorCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            groundLayer))
        {
            if (player.transform.position.x - 0.1f < transform.position.x && player.transform.position.x + 0.1f > transform.position.x)
            {
                return;
            }
            else if (player.transform.position.x > transform.position.x)
            {
                transform.position -= new Vector3(speed, 0.0f, 0.0f) * Time.deltaTime;
                transform.rotation = Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));
            }
            else if (player.transform.position.x < transform.position.x)
            {
                transform.position += new Vector3(speed, 0.0f, 0.0f) * Time.deltaTime;
                transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
            }
        }
    }

    public void TargetPlayer()
    {
        Debug.Log(currentAttackCooldown);
        if (Physics2D.OverlapArea(new Vector2(playerTrackCheck.position.x - (playerTrackCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            playerTrackCheck.position.y + (playerTrackCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            new Vector2(playerTrackCheck.position.x + (playerTrackCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            playerTrackCheck.position.y - (playerTrackCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            playerLayer) && inCooldown == false)
        {
            Instantiate(projectile, new Vector3(transform.position.x, transform.position.y + playerTrackCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2, transform.position.z), transform.rotation);
            inCooldown = true;
        }
        if (inCooldown == true)
        {
            if (currentAttackCooldown <= 0)
            {
                currentAttackCooldown = attackFrequency;
                inCooldown = false;
            }
            else
            {
                currentAttackCooldown -= 1.0f * Time.deltaTime;
            }
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
