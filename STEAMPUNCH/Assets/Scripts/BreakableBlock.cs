using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlock : MonoBehaviour
{
    // Fields ============================================================================

    [SerializeField] AudioSource sfx_breakBlock;

    // Methods ===========================================================================

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Destroys the block
    public void Break()
    {
        sfx_breakBlock.Play();

        Destroy(this.gameObject);
    }
}
