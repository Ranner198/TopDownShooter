﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeScreen : MonoBehaviour
{
    [Tooltip("The amount of time the smoke screen will go for")]
    public float timer;
    public GameObject SmokeEmiter;
    private ParticleSystem ps;
    private float inGameTimer;
    private Rigidbody rb;
    private float startDelay = 3;
    public void Start()
    {
        ps = SmokeEmiter.GetComponent<ParticleSystem>();
        rb = GetComponent<Rigidbody>();
    }

    public void LateUpdate()
    {    
        if (rb.velocity.magnitude < .1f && !ps.isPlaying && startDelay <= 0)
        {
            SmokeEmiter.transform.position = transform.position;
            SmokeEmiter.transform.rotation = Quaternion.identity;
            ps.Play();                        
        }

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
