using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCeiling : MonoBehaviour
{
    public Material baseShader, fadeShader;
 
    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<Renderer>().material = fadeShader;
        }
    }

    void OnTriggerStay (Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<Renderer>().material = fadeShader;
        }
    }
    /*
    void OnTriggerExit (Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<Renderer>().material = baseShader;
        }
    }
    */
}
