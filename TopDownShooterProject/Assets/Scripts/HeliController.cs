using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class HeliController : MonoBehaviour
{
    public GameObject parentObject;
    public List<Waypoints> waypoints = new List<Waypoints>();
    public float speed;
    [Tooltip("The distance the gameObject must be to trigger going to the next position")]
    public float errorDistance;
    public AudioClip flyAway, flyIn;
    private Rigidbody rb;
    private int index;
    private float timer;
    private AudioSource m_audio;
    private bool leaving;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        m_audio = GetComponent<AudioSource>();

        m_audio.PlayOneShot(flyIn);

        for (int i = 0; i < waypoints.Count; i++)
        {
            waypoints[i].index = i;
        }
    }

    void Update()
    {
        if (index < waypoints.Count)
        {
            if (errorDistance < Vector3.Distance(transform.position, waypoints[index].GO.transform.position))
            {
                rb.AddForce((waypoints[index].GO.transform.position - transform.position).normalized * speed * Time.deltaTime);
            }
            else if (timer >= waypoints[index].delay)
            {
                index++;
                timer = 0;
            }
            else if (timer < waypoints[index].delay)
            {
                timer += Time.deltaTime;
            }
        }        

        if (index == waypoints.Count-1 && !leaving)
        {
            m_audio.loop = false;
            m_audio.clip = null;
            m_audio.Stop();
            m_audio.PlayOneShot(flyAway);
            leaving = true;
        }

        transform.rotation = Quaternion.Euler(new Vector3(rb.velocity.z * 1.5f, 180, 0));
    }
}

[System.Serializable]
public class Waypoints 
{
    public int index;    
    public GameObject GO;
    public float delay;

    public Waypoints(int index, GameObject GO)
    {
        this.index = index;
        this.GO = GO;
    }
}

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

                int index = 0;
                foreach (Transform child in script.parentObject.transform)
                {
                    Waypoints temp = new Waypoints(index, child.gameObject);
                    script.waypoints.Add(temp);
                    index++;
                }
            }
            else
                Debug.Log("Error Parent Cannot be null try assigning it and trying again");
        }

    }

}