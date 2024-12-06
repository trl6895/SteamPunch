using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{


    [SerializeField] protected LayerMask enemyLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Damage()
    {
        if (Physics2D.OverlapArea(new Vector2(transform.position.x - (transform.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            transform.position.y + (transform.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            new Vector2(transform.position.x + (transform.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            transform.position.y - (transform.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            enemyLayer))
        {            
            Destroy(gameObject);
        }
    }


}
