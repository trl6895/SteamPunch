using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerState
{
    still,
    running,
    jumping
}


public class PlayerController : MonoBehaviour
{

    float moveForce = 5.0f;
    float jumpForce = 1.5f;
    float direction;

    PlayerState playerState;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform pos;

    // Start is called before the first frame update
    void Start()
    {
        playerState = PlayerState.still;
    }

    // Update is called once per frame
    void Update()
    {

        direction = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rb.velocity = -(new Vector2(rb.velocity.x, moveForce));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rb.velocity = new Vector2(rb.velocity.x, moveForce);
        }

    }
}
