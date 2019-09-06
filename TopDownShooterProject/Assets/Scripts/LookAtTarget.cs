using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    GameObject target = null;

    void Start()
    {
        target = GameManager.instance.gameObject;
    }

    void LateUpdate()
    {
        transform.LookAt(target.transform.position, Vector3.up);
    }
}
