using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootingController : MonoBehaviour
{
    public LayerMask lm, shootingLayer;
    public float YOffset = .5f;
    public Camera cam;
    public List<NavMeshAgent> agent = new List<NavMeshAgent>();
    public List<Animator> anim = new List<Animator>();
    public List<GameObject> player = new List<GameObject>();
    public int ammo;
    public int clipSize;
    public int clips;
    public bool isStopped;
    public static ShootingController instance;
    public AudioClip reload, shooting;
    AudioSource audioSource;
    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        lm = ~lm;  
        shootingLayer = ~shootingLayer;  
        foreach (var animationController in anim)
        {
            animationController.SetTrigger("Movement"); 
        }           
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
                        foreach (var pAgent in agent)
                        {
                            pAgent.SetDestination(hit.point);
                        }                                               
                        if (isStopped)
                        {
                            foreach (var animationController in anim)
                            {
                                animationController.SetTrigger("Movement"); 
                            }    
                        }
                        isStopped = false;
                    }
                break;
                case "Enemy":
                    if (Input.GetMouseButtonDown(1) && !isStopped)
                    {  /*
                        if (Physics.Raycast(player.transform.position, hit.transform.gameObject.transform.position - player.transform.position, out RaycastHit testHit, Mathf.Infinity, shootingLayer))
                        {
                            if(testHit.transform.tag == "Enemy")
                            {                            
                                player.transform.LookAt(hit.transform.gameObject.transform.position, Vector3.up);                        
                                isStopped = true;
                                StartCoroutine(Attacking(hit.transform.gameObject));
                            }
                            else
                            {
                                agent.SetDestination(hit.point);
                            }
                        }
                        else
                        {
                            agent.SetDestination(hit.point);                        
                        }
                        */
                    }   
                break;
            }
        }
        
        if (!isStopped)
        {                     
            //agent.isStopped = false;
            int i = 0;
            foreach (var animationController in anim)
            {
                animationController.SetFloat("XVelocity", player[i].transform.InverseTransformDirection(agent[i].velocity).x);
                animationController.SetFloat("ZVelocity", player[i].transform.InverseTransformDirection(agent[i].velocity).z);
                i++;
            }    
        }
        //else        
            //agent.isStopped = true; 
    }    

    private IEnumerator Attacking(GameObject clicked)
    {
        if (ammo > 0 && clicked.transform.GetComponent<PlayerManager>().GetHealth() > 0)
        {       
            audioSource.PlayOneShot(shooting);
            foreach (var animationController in anim)
                animationController.SetTrigger("Attacking");
            yield return new WaitForSeconds(.75f);
            ammo-=1;         
            bool dead = clicked.GetComponent<PlayerManager>().Damage(25);  
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
        audioSource.PlayOneShot(reload);
        foreach (var animationController in anim)
            animationController.SetTrigger("Reloading");
        yield return new WaitForSeconds(3);
        foreach (var animationController in anim)
            animationController.SetTrigger("Movement");
        if (clips > 0)
        {
            clips--;
            ammo = clipSize;
        }
        isStopped = false;        
    }
}
