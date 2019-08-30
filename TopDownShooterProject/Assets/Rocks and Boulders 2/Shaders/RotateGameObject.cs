using UnityEngine;
using System.Collections;

public class RotateGameObject : MonoBehaviour {
	public float rot_speed_z=0;
	
	// Use this for initialization
	void Start () { 
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Rotate(Time.fixedDeltaTime*new Vector3(0, 0, rot_speed_z), Space.Self);		
	}
}
