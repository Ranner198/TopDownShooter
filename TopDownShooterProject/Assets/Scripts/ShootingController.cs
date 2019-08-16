using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootingController : MonoBehaviour
{
    public LayerMask lm;
    public float YOffset = .5f;
    public Camera cam;
    public NavMeshAgent agent;
    void Start()
    {
        lm = ~lm;        
    }
    void Update()
    {  
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, lm))
        {                        
            if (hit.transform.tag == "Ground")
            {
                if (Input.GetMouseButtonDown(1))
                    agent.SetDestination(hit.point);
            }            
        }
    }
}
