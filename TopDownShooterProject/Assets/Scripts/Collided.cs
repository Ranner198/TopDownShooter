using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collided : MonoBehaviour
{
    public RoomClass parent;
    public GameObject roomObject;
    public int index;
    public bool destoryPositionIfUnavaliable = false;
    public void OnTriggerEnter(Collider coll)
    {    
        if (coll.gameObject != roomObject && destoryPositionIfUnavaliable && coll.gameObject.tag != "Player")
        {
            if (parent.index < coll.gameObject.transform.parent.GetComponent<RoomClass>().index)
                Destroy(coll.transform.parent.gameObject);            
        }
        else if (!destoryPositionIfUnavaliable)
        {
            parent.Deactivate(index);
            gameObject.SetActive(false);
        }        
    }
}
