using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepHealthBarInPlace : MonoBehaviour
{    
    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
