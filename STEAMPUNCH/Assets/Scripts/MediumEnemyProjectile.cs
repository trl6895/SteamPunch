using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MediumEnemyProjectile : MonoBehaviour
{

    [SerializeField] protected float damage;
    [SerializeField] protected float speed;
    [SerializeField] protected float existTime;
    [SerializeField] protected LayerMask playerLayer;
    protected GameObject player;
    protected PlayerController playerController;
    protected Transform playerPos;
    protected bool left;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        playerPos = player.GetComponent<Transform>();
        CheckDirection();
    }

    // Update is called once per frame
    void Update()
    {
        if (existTime > 0.0f)
        {
            Move();
            DamagePlayer();
        }
        else
        {
            Destroy(gameObject);
        }
    }



    public void CheckDirection()
    {
        if (player.transform.position.x > transform.position.x)
        {
            left = false;
        }
        else if (player.transform.position.x < transform.position.x)
        {
            left = true;
        }
    }

    public void Move()
    {
        if (left == true)
        {
            transform.position -= new Vector3(speed, 0.0f, 0.0f) * Time.deltaTime;
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));
        }
        else
        {
            transform.position += new Vector3(speed, 0.0f, 0.0f) * Time.deltaTime;
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
        }
        existTime -= 1.0f * Time.deltaTime;
    }

    public void DamagePlayer()
    {
        if (Physics2D.OverlapArea(new Vector2(transform.position.x - (transform.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            transform.position.y + (transform.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            new Vector2(transform.position.x + (transform.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            transform.position.y - (transform.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            playerLayer))
        {
            Debug.Log("hello");

            playerController.Health -= damage;
            playerController.InvicibilityTimer = 1f;
            //player.GetComponent<Rigidbody2D>().AddForce(new Vector2(2.0f * knockbackForce, 3.0f));
            Destroy(gameObject);
        }
    }


}
