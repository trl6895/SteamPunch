using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateSpawner : MonoBehaviour
{

    [SerializeField] private GameObject crate;
    List<GameObject> crateList;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Activate()
    {
        crateList.Add(crate);
        Instantiate(crateList[0], new Vector3(transform.position.x, transform.position.y + gameObject.GetComponent<SpriteRenderer>().bounds.size.y / 2, transform.position.z), transform.rotation);
    }

}
