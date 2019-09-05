using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class HeliController : MonoBehaviour
{
    public GameObject parentObject;
    public List<Waypoints> waypoints = new List<Waypoints>();
    public float duration = 5.0f;
    [Tooltip("The distance the gameObject must be to trigger going to the next position")]
    public float errorDistance;
    public AudioClip flyAway, flyIn;
    private int index;
    private float timer;
    private AudioSource m_audio;
    private bool leaving;
    private float minimum = 0.001f;
    private float maximum = 1.0f;
    float startTime;
    void Start()
    {
        m_audio = GetComponent<AudioSource>();

        m_audio.PlayOneShot(flyIn);

        for (int i = 0; i < waypoints.Count; i++)
        {
            waypoints[i].index = i;
        }

        startTime = Time.time;
    }

    void Update()
    {
        float t = (Time.time - startTime) / duration;

        if (index < waypoints.Count)
        {
            if (errorDistance < Vector3.Distance(transform.position, waypoints[index].GO.transform.position))
            {                
                transform.position = Vector3.Lerp(transform.position, waypoints[index].GO.transform.position, Mathf.SmoothStep(minimum, maximum, t));
            }
            else if (timer >= waypoints[index].delay)
            {            
                if (waypoints[index].spawn)
                    GameManager.instance.SpawnFriendlies();    
                index++;
                startTime = Time.time-Time.deltaTime;
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

        //transform.rotation = Quaternion.Euler(new Vector3(transform.velocity.z * 1.5f, 180, 0));
    }
}

[System.Serializable]
public class Waypoints 
{
    public int index;    
    public GameObject GO;
    public float delay;
    public bool spawn;

    public Waypoints(int index, GameObject GO)
    {
        this.index = index;
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
#endif