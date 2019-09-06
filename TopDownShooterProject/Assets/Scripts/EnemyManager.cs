using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine.UI;
public class EnemyManager : MonoBehaviour
{
    // Public Variables
    public bool alive {get; set;}
    public bool alerted {get; set;}
    public bool patrol {get; set;}
    public float senseDistance {get; set;}    
    public float stoppingPosition;
    public bool dead {get; set;}
    public int health = 100;
    public bool stationary;
    // Gun Logic
    public int ammo {get; set;}
    public int ammoSize = 25;
    //Sound
    public AudioClip shoot, reload, walk;
    public new AudioSource audio;    
    // Navmesh Agent
    public NavMeshAgent agent;    
    // Vision Variables
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    // LayerMasks
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public LayerMask lm;
    // State Variables
    public enum State {Passive, Attacking, Reloading};
    public State state;
    // UI
    public Slider healthBar;

    // Targets
    public List<GameObject> visibleTargets = new List<GameObject>();
    private GameObject target;
    // Animator
    private Animator anim;
    // Colliders
    private BoxCollider BC;
    
    void Start()
    {    
        anim = GetComponent<Animator>();     
        patrol = true;
        StartCoroutine(FindTargetsWithDelay(.2f));
        BC = GetComponent<BoxCollider>();
        ammo = ammoSize;
        if (!stationary)
            agent.SetDestination(RandomNavmeshLocation(20));    
    }
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            if (target == null)
            {                
                FindVisibleTargets();
            }          
        }
    }
    void Update()
    {
        anim.SetFloat("XVelocity", anim.gameObject.transform.InverseTransformDirection(agent.velocity).x);
        anim.SetFloat("ZVelocity", anim.gameObject.transform.InverseTransformDirection(agent.velocity).z);
    }
    void FindVisibleTargets()
    {   
        if (!stationary && (agent.destination - transform.position).magnitude < stoppingPosition)            
            agent.SetDestination(RandomNavmeshLocation(20));
             
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius);

        if (visibleTargets.Count > 0)
            visibleTargets.Clear();

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
           Transform targetTransform = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (targetTransform.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, targetTransform.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    visibleTargets.Add(targetTransform.gameObject);
                    
                    foreach(GameObject visibleTarget in visibleTargets)
                    {
                        if (visibleTarget.tag == "Player")
                        {
                            print("Bruh");
                            target = visibleTarget.gameObject;
                            Attack(target);
                            return;
                        }
                        else
                            target = null;
                    }
                }
            }
        }
    }
    public Vector3 RandomNavmeshLocation(float radius) {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
            finalPosition = hit.position;            
        }
        else
            return transform.position;

        return finalPosition;
     }
    IEnumerator Steer(float angle)
    {
        float timer = 0;
        while(!GlobalMethods.Epsilon(transform.rotation.eulerAngles.y, angle))
        {
            Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, angle, transform.rotation.z), timer);
            timer += Time.deltaTime/3;
            yield return new WaitForEndOfFrame();
        }
    }
    public bool Damage(int amt)
    {
        health -= amt;
        healthBar.value = health;
        CheckHealth();        
        return health <= 0;
    }
    public void CheckHealth()
    {
        if (health <= 0 && !dead)
        {
            print("I died");
            agent.isStopped = true;
            BC.enabled = false;
            dead = true;
            anim.SetTrigger("Death");   
            GameManager.instance.PopEnenmy(gameObject);
           healthBar.gameObject.SetActive(false);
            Destroy(this); 
        }
    }
    public int GetHealth()
    {
        return this.health;
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
     public void Attack(GameObject go)
    {
        if (Physics.Raycast(transform.position, (go.transform.position - transform.position).normalized, out RaycastHit hit, Mathf.Infinity, lm))
        {
            print(hit.transform.name);
            if (hit.transform.tag == "Player")
            {
                agent.isStopped = true;
                target = go;
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
                target = null;                    
                StartCoroutine(Wait(.75f, go));
            }
        }

        Debug.DrawRay(transform.position, (go.transform.position - transform.position).normalized * 8, Color.red, 2, true);
    }

    IEnumerator Wait(float amt, GameObject go)
    {
        yield return new WaitForSeconds(amt);
        if (go.GetComponent<EnemyManager>().dead)
        {
            yield break;
        }
        else
        {
            Attack(go);
        }
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
            try 
            {
                if (target.GetComponent<PlayerManager>().Damage(15))
                {
                    target = null;
                    Reload();
                }
            }
            catch(System.Exception)
            {
                target = null;
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
        if (target == null)
            state = State.Passive;
        else
            Attack(target);
    }
    // Footstep should occur
    public void Footstep()
    {
        // Unity is bad so we must only try to run it half the time or the audio will break
        if (Random.Range(0f, 1f) > .75f)
            audio.PlayOneShot(walk);
    }
    
	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
}
#if UNITY_EDITOR
[CustomEditor(typeof(EnemyManager))]
public class EnenmyVision : Editor 
{
	void OnSceneGUI() {

		EnemyManager fow = (EnemyManager)target;
        Handles.color = Color.red; 

        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);

        Vector3 viewAngleA = fow.DirFromAngle(-(fow.viewAngle / 2), false);
        Vector3 viewAngleB = fow.DirFromAngle( (fow.viewAngle / 2), false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

		Handles.color = Color.red;
		foreach (GameObject visibleTarget in fow.visibleTargets) {
			Handles.DrawLine (fow.transform.position, visibleTarget.transform.position);
		}
	}
}
#endif