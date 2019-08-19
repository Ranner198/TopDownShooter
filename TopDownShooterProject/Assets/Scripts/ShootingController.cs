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
    public Animator anim;
    public GameObject player;
    public int ammo;
    public int clipSize;
    public int clips;
    public bool isStopped;
    public GameObject Hovered;
    public static ShootingController instance;
    public GameObject ps;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        lm = ~lm;    
        anim.SetTrigger("Movement");    
    }
    void Update()
    {  
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, lm))
        {             
            switch (hit.transform.tag)
            {
                case "Ground":
                    if (Input.GetMouseButtonDown(1))
                    {
                        StopAllCoroutines();                        
                        agent.SetDestination(hit.point);                        
                        if (isStopped)
                        {
                            anim.SetTrigger("Movement");
                            ps.SetActive(false);
                        }
                        isStopped = false;
                    }
                break;
                case "Enemy":
                    if (Input.GetMouseButtonDown(1) && !isStopped)
                    {  
                        player.transform.LookAt(hit.transform.position, Vector3.up);                    
                        isStopped = true;
                        StartCoroutine(Attacking(hit.transform.gameObject));
                    }   
                break;
            }
        }

        if (!isStopped)
        {                     
            agent.isStopped = false;
            anim.SetFloat("XVelocity", player.transform.InverseTransformDirection(agent.velocity).x);
            anim.SetFloat("ZVelocity", player.transform.InverseTransformDirection(agent.velocity).z);
        }
        else        
            agent.isStopped = true; 

        /*
        if (Hovered != null)
        {
            print(agent.velocity);
            agent.SetDestination(Hovered.transform.position);
            anim.SetFloat("XVelocity", player.transform.InverseTransformDirection(agent.velocity).x);
            anim.SetFloat("ZVelocity", player.transform.InverseTransformDirection(agent.velocity).z);
        }
        */
    }    

    private IEnumerator Attacking(GameObject clicked)
    {
        if (ammo > 0 && clicked.transform.GetComponent<Health>().GetHealth() > 0)
        {       
            ps.SetActive(true);  
            anim.SetTrigger("Attacking");
            yield return new WaitForSeconds(.75f);
            ammo-=1;         
            bool dead = clicked.GetComponent<Health>().Damage(25);  
            if (dead)
            {
                clicked.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Death");
                StartCoroutine(Reload());
            }
            else
                StartCoroutine(Attacking(clicked));
        }
        else
            StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        ps.SetActive(false);      
        anim.SetTrigger("Reloading");
        yield return new WaitForSeconds(3);
        if (clips > 0)
        {
            clips--;
            ammo = clipSize;
        }
        isStopped = false;
    }
}
