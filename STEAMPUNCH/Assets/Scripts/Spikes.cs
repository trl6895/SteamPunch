using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Spikes : MonoBehaviour
{
    // Fields ===========================================================================

    [SerializeField] private BoxCollider2D hitbox;

    // Methods =======================================================================

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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

            // If the current overlapping collider belongs to the player:
            if (contacts[i].gameObject.TryGetComponent<PlayerController>(out tempPlayer))
            {
                tempPlayer.Health--;
            }
            // Otherwise, if the current overlapping collider belongs to an enemy:
            else if (contacts[i].gameObject.TryGetComponent<Enemy>(out tempEnemy))
            {
                // If the enemy is alive:
                if (tempEnemy.CurrentState == EnemyStates.Alive)
                {
                    //Damage the enemy and make it jump a little bit
                    tempEnemy.Punched(new Vector2(0.0f, 600.0f));
                }
            }
        }
    }
}
