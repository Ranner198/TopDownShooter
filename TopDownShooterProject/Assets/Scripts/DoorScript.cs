using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 startingLocation;
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = transform.position;
        startingLocation = transform.position;
    }
    
    public void Update()
    {
        transform.position = startingLocation;
    }
}
