using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeScreen : MonoBehaviour
{
    [Tooltip("The amount of time the smoke screen will go for")]
    public float timer;
    public GameObject SmokeEmiter;
    public float duration;
    private ParticleSystem ps;
    private float inGameTimer;
    private Rigidbody rb;
    private float startDelay = 3;
    public Vector3 Target;
    public float firingAngle = 45.0f;
    public float gravity = 9.8f;
    public Transform Projectile;      
    private Transform myTransform;
    void Awake()
    {
        myTransform = transform;      
    }
    public void Start()
    {
        ps = SmokeEmiter.GetComponent<ParticleSystem>();
        rb = GetComponent<Rigidbody>();        
    }
    public void Huck()
    {
        StartCoroutine(SimulateProjectile());
    }
    IEnumerator SimulateProjectile()
    {
        // Short delay added before Projectile is thrown
        yield return new WaitForEndOfFrame();
       
        // Move projectile to the position of throwing object + add some offset if needed.
        Projectile.position = myTransform.position + new Vector3(0, 0.0f, 0);    

        // Calculate distance to target
        float target_Distance = Vector3.Distance(Projectile.position, Target);
 
        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);
 
        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
 
        // Calculate flight time.
        float flightDuration = target_Distance / Vx;
   
        // Rotate projectile to face the target.
        Projectile.rotation = Quaternion.LookRotation(Target - Projectile.position);
       
        float elapse_time = 0;
 
        while (elapse_time < flightDuration)
        {
            Projectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
           
            elapse_time += Time.deltaTime;
 
            yield return null;
        }

        rb.useGravity = true;
        ps.Play();          
    }  
    public void LateUpdate()
    {    
        // Update the position of the smoke so it matches the canister
        SmokeEmiter.transform.position = transform.position;
        SmokeEmiter.transform.rotation = Quaternion.identity;
        
        if (startDelay > 0)
        {
            startDelay -= Time.deltaTime;
        }

        if (inGameTimer < timer)
        {
            inGameTimer += Time.deltaTime;
        }

        if (inGameTimer >= timer)
        {
            ps.Stop();
            Destroy(gameObject, 5f);
        }
    }    
}