using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{      
    public int index;
    public GameObject[] players;
    public GameObject temp;
    public float cameraHeight = 8.95f, maxHeight = 20, minHeight = 2;
    public float Zdist = -5.75f;        
    public static CameraFollow instance;

    void Awake()
    {
        instance = this;
    }
    public void Resample() 
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
    void Update()
    {
        float amt = -Input.mouseScrollDelta.y;

        if (amt > 0 && cameraHeight < maxHeight)
        {
            cameraHeight += amt;
        }
        else if (amt < 0 && cameraHeight > minHeight)
        {
            cameraHeight += amt;
        }

        if (cameraHeight > maxHeight)
            cameraHeight = maxHeight;
        if (cameraHeight < minHeight)
            cameraHeight = minHeight;
    }

    void LateUpdate() {
        if (players.Length > 0)
        {        
            Vector3 pos = players[index].transform.position;
            pos.z += Zdist;
            pos.y = players[index].transform.position.y + cameraHeight;
            transform.position = pos;
        }
        else
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            if (temp != null)
            {
                Vector3 pos = temp.transform.position;
                pos.z += Zdist;
                pos.y = temp.transform.position.y + cameraHeight;
                transform.position = pos;
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (index <= players.Length-1)
                index++;
            else
                index = 0;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (index >= 0)
                index--;
            else
                index = 0;
        }
    }
}
