using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerManager : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator anim;

    // Player Variables
    public float health {get; set;}
    public float accuracy {get; set;}    
    public int index {get; set;}

    // Gun Logic
    public int ammo {get; set;}
    public int ammoSize = 25;

    //Sound
    public AudioClip shoot, reload, walk;
    public new AudioSource audio;

    // Target for attacking
    public GameObject Target;

    // State Variables
    public enum State {Passive, Attacking, Reloading};
    public State state;

    public LayerMask lm;

    // Constructor, idk why this is here tbh
    public PlayerManager()
    {

    }

    public void Start()
    {
        state = State.Passive;
        ammo = ammoSize;
        lm = ~lm;        
    }

    // Destructor, Our player has died
    public void KillPlayer()
    {
        agent.speed = 0;
        health = 0;
        gameObject.layer = 2;        
        anim.SetTrigger("Death");
    }

    // If we are moving and not attacking play the moving animations
    public void Update()
    {
        if (state == State.Passive)
        {
            anim.SetFloat("XVelocity", anim.gameObject.transform.InverseTransformDirection(agent.velocity).x);
            anim.SetFloat("ZVelocity", anim.gameObject.transform.InverseTransformDirection(agent.velocity).z);
        }
    }

    // Repaint the animations controller
    public void UpdateAnimationController()
    {
        switch (state)
        {
            case State.Attacking:
                anim.Play("Attack",-1,Random.Range(0f, 0.99f));
            break;
            case State.Reloading:
                anim.Play("Reloading",-1,Random.Range(0f, 0.3f));
            break;
        }
    }

    // We need to move to the passed location
    public void MoveTo(Vector3 pos)
    {                
        agent.SetDestination(pos);    
        state = State.Passive;        
        anim.Play("Blend Tree",-1,Random.Range(0f, 0.99f));
        agent.isStopped = false;
        Target = null;    
        agent.stoppingDistance = 0.15f + index;
    }

    // Start attacking the passed gameobject if we can see them else go towards them
    public void Attack(GameObject go)
    {
        if (Physics.Raycast(transform.position, (go.transform.position - transform.position).normalized, out RaycastHit hit, Mathf.Infinity, lm))
        {
            print(hit.transform.name);
            if (hit.transform.tag == "Enemy")
            {
                agent.isStopped = true;
                Target = go;
                state =  State.Attacking;
                transform.LookAt(go.transform.position);
                UpdateAnimationController();
            }
            else
            {
                agent.SetDestination(go.transform.position);    
                state = State.Passive;        
                anim.Play("Blend Tree",-1,Random.Range(0f, 0.99f));
                agent.isStopped = false;
                Target = null;                    
                StartCoroutine(Wait(.75f, go));
            }
        }

        Debug.DrawRay(transform.position, (go.transform.position - transform.position).normalized * 8, Color.red, 2, true);
    }

    IEnumerator Wait(float amt, GameObject go)
    {
        yield return new WaitForSeconds(amt);
        Attack(go);
    }

    // Reload Method
    public void Reload()
    {
        agent.isStopped = true;
        state =  State.Reloading;
        UpdateAnimationController();
    }

    // Sound Controllers ---------------------------------------
    // This stuff runs off of Unity events in the animation controller

    // Shoot the gun
    public void Shoot()
    {        
        //  Does our gun have ammo?
        if (ammo > 0)
        {
            audio.PlayOneShot(shoot);
            if (Target.GetComponent<Health>().Damage(15))
            {
                Target = null;
                Reload();
            }
        }
        else       // No? Reload then.
            Reload();
    }

    // Start Reload Sound Effect
    public void ReloadEffect()
    {
        audio.PlayOneShot(reload);
    }
    // We Finished Reloading
    public void FinishReload()
    {
        // Reload Mag
        ammo = ammoSize;

        // Are we still attacking?
        if (Target == null)
            state = State.Passive;
        else
            Attack(Target);
    }
    // Footstep should occur
    public void Footstep()
    {
        // Unity is bad so we must only try to run it half the time or the audio will break
        if (Random.Range(0f, 1f) > .75f)
            audio.PlayOneShot(walk);
    }
}
