using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeScreen : Throwable
{
    [Tooltip("The amount of time the smoke screen will go for")]
    public float timer;
    public GameObject SmokeEmiter;
    public float duration;
    private ParticleSystem ps;
    private float inGameTimer;
    private Rigidbody rb;
    private float startDelay = 3;
    public void Start()
    {
        ps = SmokeEmiter.GetComponent<ParticleSystem>();
        rb = GetComponent<Rigidbody>();        
    }
    public override void AudioCallout()
    {
        AudioManger.instance.Play("RequestingEvac");
    }
    public override void Landed()
    {
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