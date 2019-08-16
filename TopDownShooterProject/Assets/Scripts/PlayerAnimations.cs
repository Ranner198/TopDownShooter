using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimations : MonoBehaviour
{
    public Animator anim;
    public NavMeshAgent agent;

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("XVelocity", transform.InverseTransformDirection(agent.velocity).x);
        anim.SetFloat("ZVelocity", transform.InverseTransformDirection(agent.velocity).z);
    }
}

