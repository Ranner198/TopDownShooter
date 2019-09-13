using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RopeControllerSimple : MonoBehaviour
{
    public int numberOfVertices = 3;
    public GameObject child;
    private LineRenderer lr;
    void Start()
    {        
        lr = GetComponent<LineRenderer>();
        lr.positionCount = numberOfVertices+2;
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, Vector3.zero);
        
        if (numberOfVertices > 0)
        {
            for (int i = 1; i < lr.positionCount-2; i++)
            {
                lr.SetPosition(i, BezierPosition(child.transform.localPosition, transform.position, i/numberOfVertices));
            }
        }

        lr.SetPosition(lr.positionCount-1, child.transform.localPosition);
    }

    public void StartWinch()
    {}

    public Vector3 BezierPosition(Vector3 A, Vector3 B, float t)
    {
        return (Mathf.Pow(1-t, 2) * A + (2*(1-t))*(t*B) + (t*t)*B);
    }
}

