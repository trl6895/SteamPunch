using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    PlayerController player;

    [SerializeField]
    private Sprite zero;
    [SerializeField]
    private Sprite one;
    [SerializeField]
    private Sprite two;
    [SerializeField]
    private Sprite three;
    [SerializeField]
    private Sprite four;
    [SerializeField]
    private Sprite five;
    [SerializeField]
    private Sprite six;
    [SerializeField]
    private Sprite seven;
    [SerializeField]
    private Sprite eight;
    [SerializeField]
    private Sprite nine;
    [SerializeField]
    private Sprite ten;

    // Update is called once per frame
    void Update()
    {
        switch (player.Health)
        {
            case 0:
                {
                    gameObject.GetComponent<Image>().sprite = zero;
                    break;
                }
            case 1:
                {
                    gameObject.GetComponent<Image>().sprite = one;
                    break;
                }
            case 2:
                {
                    gameObject.GetComponent<Image>().sprite = two;
                    break;
                }
            case 3:
                {
                    gameObject.GetComponent<Image>().sprite = three;
                    break;
                }
            case 4:
                {
                    gameObject.GetComponent<Image>().sprite = four;
                    break;
                }
            case 5:
                {
                    gameObject.GetComponent<Image>().sprite = five;
                    break;
                }
            case 6:
                {
                    gameObject.GetComponent<Image>().sprite = six;
                    break;
                }
            case 7:
                {
                    gameObject.GetComponent<Image>().sprite = seven;
                    break;
                }
            case 8:
                {
                    gameObject.GetComponent<Image>().sprite = eight;
                    break;
                }
            case 9:
                {
                    gameObject.GetComponent<Image>().sprite = nine;
                    break;
                }
            case 10:
                {
                    gameObject.GetComponent<Image>().sprite = ten;
                    break;
                }
        }
            
    }
}
