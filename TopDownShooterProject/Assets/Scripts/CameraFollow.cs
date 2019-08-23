using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{      
    public GameObject player;
    public float cameraHeight = 8.95f;
    public float Zdist = -5.75f;    

    void LateUpdate() {
        if (player != null)
        {
            Vector3 pos = player.transform.position;
            pos.z += Zdist;
            pos.y = player.transform.position.y + cameraHeight;
            transform.position = pos;
        }
        else
            player = GameObject.FindGameObjectWithTag("Player");
    }
}
