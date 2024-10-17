using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallEnemy : Enemy
{


    [SerializeField] private float speed;
    private float walkDistance;
    [SerializeField] private float setWalkDistance;
    [SerializeField] private Transform collisionCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool facingLeft;
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

        switch (CurrentState)
        {
            case EnemyStates.Alive:
                Walk();
                break;
            case EnemyStates.Knocked:
                break;
            case EnemyStates.Grabbed:
                break;
            default:
                break;
        }

    }

    public void Walk()
    {

        if (Physics2D.OverlapArea(new Vector2(collisionCheck.position.x - (collisionCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            collisionCheck.position.y + (collisionCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            new Vector2(collisionCheck.position.x + (collisionCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            collisionCheck.position.y - (collisionCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            groundLayer) && walkDistance >= 0.0f)
        {
            transform.position -= new Vector3(speed, 0.0f, 0.0f) * Time.deltaTime;
            walkDistance -= Mathf.Abs(speed) * Time.deltaTime;
        }
        else
        {
            transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
            speed *= -1;
            walkDistance = setWalkDistance;
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
