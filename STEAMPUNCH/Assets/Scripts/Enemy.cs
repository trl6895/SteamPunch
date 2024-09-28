using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates 
{
    Alive,
    Knocked
}

public class Enemy : MonoBehaviour
{

    // This field is serialized atm for testing purposes
    [SerializeField]
    EnemyStates currentState;

    private SpriteRenderer sprite;

    // Right now my idea for this is that an enemy needs to be
    // "Knocked" for it to be grabbed, so the playee script will have
    // to use this getter to determine if an enemy is "Knocked" when
    // attempting to pick it up
    public EnemyStates CurrentState { get { return currentState; } }

    // Start is called before the first frame update
    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == EnemyStates.Alive)
        {
            sprite.color = Color.white;
            // do alive stuff
        }
        // making this an else if for the time being, 
        // in case we add more states
        else if (currentState == EnemyStates.Knocked) 
        {
            sprite.color = Color.red;
            // do "dead" stuff
        }
    }
}
