using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{      
    public GameObject player;
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
        player = GameObject.FindGameObjectWithTag("Player");
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
        if (player != null)
        {        
            Vector3 pos = player.transform.position;
            pos.z += Zdist;
            pos.y = player.transform.position.y + cameraHeight;
            transform.position = pos;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (temp != null)
            {
                Vector3 pos = temp.transform.position;
                pos.z += Zdist;
                pos.y = temp.transform.position.y + cameraHeight;
                transform.position = pos;
            }
        }
    }
}
