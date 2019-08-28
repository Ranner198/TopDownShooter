using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomClass : MonoBehaviour
{
    public List<GameObject> doors = new List<GameObject>();
    public List<GameObject> spawnPoints = new List<GameObject>();
    private List<BoxCollider> boxColliders = new List<BoxCollider>();
    public List<GameObject> EnemySpawnPoints = new List<GameObject>();
    public bool isSpawning = true;
    public int index;
    public void Start()
    {
        foreach (GameObject spawn in spawnPoints)
            boxColliders.Add(spawn.GetComponent<BoxCollider>());
    }
    public List<GameObject> GetWalls() 
    {
        return doors;
    }
    public List<GameObject> GetSpawnPoints() 
    {
        return spawnPoints;
    } 
    public List<GameObject> GetEnemySpawnPoints()
    {
        return EnemySpawnPoints;
    }
    public void Deactivate(int index)
    {  
        if (isSpawning)      
            doors[index].SetActive(false);
    }    
    public GameObject GetDoor(int i)
    {
        return doors[i];
    }    
}
