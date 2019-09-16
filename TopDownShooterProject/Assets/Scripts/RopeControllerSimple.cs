using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeControllerSimple : MonoBehaviour
{
    public int numberOfVertices = 3;
    public GameObject child;
    private LineRenderer lr;
    void Start()
    {        
        lr = GetComponent<LineRenderer>();
        //lr.positionCount = numberOfVertices+2;
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, transform.position);
        
        /*
        if (numberOfVertices > 0)
        {
            for (int i = 1; i < lr.positionCount-2; i++)
            {
                
            }
        }
        */

        //lr.SetPosition(1, BezierPosition(child.transform.localPosition, transform.position, .5f));

        //print(BezierPosition(child.transform.localPosition, transform.position, .5f));

        lr.SetPosition(1, child.transform.position);
    }

    public void StartWinch()
    {}

	public static Vector3 BezierPosition (Vector3 A, Vector3 B, float t) {

        Vector3 C = (A + B)/2; // Sample a mid point

		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;

		return oneMinusT * oneMinusT * A + 2f * oneMinusT * t * C + t * t * B;
	}
}

