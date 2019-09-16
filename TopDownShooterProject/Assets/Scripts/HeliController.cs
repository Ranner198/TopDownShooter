﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class HeliController : MonoBehaviour
{
    public GameObject parentObject;
    public List<Waypoints> waypoints = new List<Waypoints>();
    public List<GameObject> LZRopes = new List<GameObject>();
    public float speed;
    [Tooltip("The distance the gameObject must be to trigger going to the next position")]
    public float errorDistance;
    public AudioClip flyAway, flyIn;
    private Rigidbody rb;
    private int m_index;
    private float timer;
    private AudioSource m_audio;
    private bool leaving;
    public bool pickingUp;
    public BoxCollider bc;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        m_audio = GetComponent<AudioSource>();

        m_audio.PlayOneShot(flyIn);

        for (int i = 0; i < waypoints.Count; i++)
        {
            waypoints[i].m_index = i;
        }
    }

    public void PickUpPlayer(Vector3 position)
    {
        position.y += 40f;
        
        // Move to landing position
        transform.position = position;
        pickingUp = true;

        // Start Audio
        m_audio.loop = true;
        m_audio.clip = flyIn;
        m_audio.Play();
        m_audio.volume = 1f;

        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        bc.enabled = true;

        StartCoroutine(Landing());
    }

    IEnumerator Landing()
    {
        while (transform.position.y > 2)
        {
            Vector3 height = transform.position;
            height.y -= 9.81f * Time.deltaTime;
            transform.position = height;

            yield return new WaitForEndOfFrame();
        }
    }

    void Update()
    {
        if (m_index < waypoints.Count && !pickingUp)
        {
            if (errorDistance < Vector3.Distance(transform.position, waypoints[m_index].GO.transform.position))
            {
                rb.AddForce((waypoints[m_index].GO.transform.position - transform.position).normalized * speed * Time.deltaTime);
            }
            else if (timer >= waypoints[m_index].delay)
            {
                if (waypoints[m_index].spawn)
                    GameManager.instance.SpawnFriendlies();    
                if (waypoints[m_index].dropRope)
                {
                    foreach (GameObject rope in LZRopes)
                    {
                        rope.SetActive(true);
                    }
                }
                if (waypoints[m_index].pullRopeIn)
                {
                    foreach (GameObject rope in LZRopes)
                    {
                        //rope.GetComponent<RopeControllerSimple>().StartWinch();
                        rope.GetComponent<RopeControllerSimple>().child.transform.position = rope.transform.position;
                        rope.GetComponent<RopeControllerSimple>().child.GetComponent<Rigidbody>().useGravity = false;
                    }
                }
                m_index++;
                timer = 0;             
            }
            else if (timer < waypoints[m_index].delay)
            {
                timer += Time.deltaTime;
            }
        }        

        if (m_index == waypoints.Count-1 && !leaving)
        {
            m_audio.loop = false;
            m_audio.clip = null;
            m_audio.Stop();
            m_audio.PlayOneShot(flyAway);
            leaving = true;
        }

        transform.rotation = Quaternion.Euler(new Vector3(-rb.velocity.z * 1.5f, 180, 0));
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player" && GameManager.instance.MissionComplete)
        {
            GameManager.instance.RemovePlayer(coll.gameObject.GetComponent<PlayerManager>().index);
            Destroy(coll.gameObject);
            GameManager.instance.CompelteMission();        
        }
    }
}

[System.Serializable]
public class Waypoints 
{
    public int m_index;    
    public GameObject GO;
    public float delay;
    public bool spawn;
    public bool dropRope;
    public bool pullRopeIn;

    public Waypoints(int m_index, GameObject GO)
    {
        this.m_index = m_index;
        this.GO = GO;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(HeliController))]
public class HeliControllerGUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HeliController script = target as HeliController;

        if (GUILayout.Button("Auto Assign List"))
        {
            if (script.parentObject != null)
            {
                if (script.waypoints.Count > 0)
                    script.waypoints.Clear();

                int m_index = 0;
                foreach (Transform child in script.parentObject.transform)
                {
                    Waypoints temp = new Waypoints(m_index, child.gameObject);
                    script.waypoints.Add(temp);
                    m_index++;
                }
            }
            else
                Debug.Log("Error Parent Cannot be null try assigning it and trying again");
        }
    }
}
#endif