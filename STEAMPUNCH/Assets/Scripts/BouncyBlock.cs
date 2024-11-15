using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBlock : MonoBehaviour
{
    // Fields ============================================================================

    [SerializeField] private BoxCollider2D hitbox;

    // Methods ===========================================================================

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision");

        // Initialize a list to hold all colliders that are colliding with the spikes
        List<Collider2D> contacts = new List<Collider2D>();

        // Fill the list with all contacts
        hitbox.OverlapCollider(new ContactFilter2D().NoFilter(), contacts);

        // For each contact point in the punch collider:
        for (int i = 0; i < contacts.Count; i++)
        {
            // Create a temporary player reference
            PlayerController tempPlayer;

            // Create a temporary enemy reference
            Enemy tempEnemy;

            Debug.Log(contacts[i]);

            // If the current overlapping collider belongs to the player:
            if (contacts[i].gameObject.TryGetComponent<PlayerController>(out tempPlayer))
            {
                // If the player is colliding from the top:
                if(tempPlayer.hitbox.transform.position.y > hitbox.transform.position.y + (hitbox.size.y / 2.0f))
                {
                    tempPlayer.rb.velocity += new Vector2(0.0f, collision.relativeVelocity.y * -0.75f);
                }
                //// Otherwise, if the player is colliding from the left:
                //else if(tempPlayer.hitbox.transform.position.x + (tempPlayer.hitbox.size.x / 2.0f) < hitbox.transform.position.x - (hitbox.size.x / 2.0f))
                //{
                //    tempPlayer.rb.velocity += new Vector2(-100.0f, 0.0f);
                //    Debug.Log($"left, {collision.relativeVelocity}");
                //}
                //// Otherwise, if the player is colliding from the right:
                //else if (tempPlayer.hitbox.transform.position.x - (tempPlayer.hitbox.size.x / 2.0f) > hitbox.transform.position.x + (hitbox.size.x / 2.0f))
                //{
                //    tempPlayer.rb.velocity += new Vector2(collision.relativeVelocity.x * 0.75f, 0.0f);
                //    Debug.Log($"right, {collision.relativeVelocity}");
                //}
                // Otherwise, if the player is colliding from the bottom:
                else if (tempPlayer.hitbox.transform.position.y + (tempPlayer.hitbox.size.y) < hitbox.transform.position.y - (hitbox.size.y / 2.0f))
                {
                    tempPlayer.rb.velocity += new Vector2(0.0f, collision.relativeVelocity.y * -1.333f);
                }
            }
            //// Otherwise, if the current overlapping collider belongs to an enemy:
            //else if (contacts[i].gameObject.TryGetComponent<Enemy>(out tempEnemy))
            //{
            //    Debug.Log($"{tempEnemy.BoxCollider.transform.position.y} > {hitbox.transform.position.y + (hitbox.size.y / 2.0f)}");
            //    // If the enemy is colliding from the top:
            //    if (tempEnemy.BoxCollider.transform.position.y > hitbox.transform.position.y + (hitbox.size.y / 2.0f))
            //    {
            //        tempEnemy.rb.velocity += new Vector2(0.0f, collision.relativeVelocity.y * -0.75f);
            //        Debug.Log("top");
            //    }
            //    // Otherwise, if the enemy is colliding from the bottom:
            //    else if (tempEnemy.BoxCollider.transform.position.y + (tempEnemy.BoxCollider.size.y) < hitbox.transform.position.y - (hitbox.size.y / 2.0f))
            //    {
            //        tempEnemy.rb.velocity += new Vector2(0.0f, collision.relativeVelocity.y * -1.333f);
            //    }
            //}
        }
    }
}
