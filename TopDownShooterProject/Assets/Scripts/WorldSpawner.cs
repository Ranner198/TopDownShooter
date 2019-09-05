using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class WorldSpawner : MonoBehaviour
{
    public int amt;
    public List<RoomClass> spawnedObjects = new List<RoomClass>();
    public List<GameObject> spawnPoints = new List<GameObject>();
    public List<Vector3> spawnedPositions = new List<Vector3>();
    public GameObject enemyPrefab;
    public GameObject spawnPeice;
    public bool runOnAwake = false;

    void Awake()
    {
        if (runOnAwake)
        {
            StartCoroutine(BuildWorld());
        }
    }

    IEnumerator BuildWorld()
    {        
        while (spawnedObjects.Count < amt)
        {
            // New GameObject Allocation
            GameObject spanwed = null;

            int rand = Random.Range(0, spawnPoints.Count-1);

            // If there is a spawn point try to spawn on it
            if (spawnedObjects.Count > 0)
            {                                
                spanwed = Instantiate(spawnPeice, Vector3.zero, Quaternion.identity);   

                if (spawnPoints.Count > 0 && spawnPoints[rand] == null)
                {
                    spawnPoints.RemoveAt(rand);
                    while (spawnPoints[rand] == null)
                    {
                        spawnPoints.RemoveAt(rand);
                        rand = Random.Range(0, spawnPoints.Count-1);
                    }
                }
                if (spawnPoints.Count > 0 && spawnPoints[rand] != null)
                {
                    spanwed.transform.position = spawnPoints[rand].transform.position;
                    spawnPoints.RemoveAt(rand);   
                }

            }
            else
                spanwed = Instantiate(spawnPeice, transform.position, Quaternion.identity);            

            var instance = spanwed.GetComponent<RoomClass>();

            instance.index = spawnedObjects.Count;

            spanwed.name = "Room: " + spawnedObjects.Count;
        
            yield return new WaitForSeconds(.0001f);
            
            if (instance != null)
            {
                spawnedObjects.Add(spanwed.GetComponent<RoomClass>());            

                // Add the possible spawn points    
                foreach (GameObject spawnPoint in instance.spawnPoints)
                {
                    spawnPoints.Add(spawnPoint);
                }

                spanwed.transform.SetParent(transform);

                spanwed = null;
            }
            else
            {
                spawnedObjects.RemoveAt(spawnedObjects.Count-1);             
            }    

            if (spawnPoints.Count > 0)
                UpdateSpawnPoints();
        }
        
        Bake();
    }

    public void Bake()
    {
        //UnityEditor.AI.NavMeshBuilder.BuildNavMesh(); 

        SpawnEnenmies();
    }

    public void SpawnEnenmies()
    {        
        // Spawn Enemies
        int i = 0;
        foreach(RoomClass pos in spawnedObjects)
        {
            pos.isSpawning = false;
            foreach(GameObject spawnPosition in pos.GetEnemySpawnPoints())
            {                
                if (Random.Range(0f, 1f) > .75f && i > 0)
                {                    
                    Instantiate(enemyPrefab, spawnPosition.transform.position, Quaternion.identity);
                }
            }

            i++;
        }
    }

    public void UpdateSpawnPoints()
    {
        for (int i = spawnPoints.Count-1; i > 0; i--)
        {
            if (spawnPoints[i] == null || !spawnPoints[i].activeSelf)
                spawnPoints.RemoveAt(i);                            
        }
    }

    // Destory all of the parts
    public void DeleteWorld()
    {
        while (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.transform.gameObject);
            }
        }

        if (spawnedObjects.Count > 0)
            spawnedObjects.Clear();
        if(spawnPoints.Count > 0)
            spawnPoints.Clear();
    }    
}

#if UNITY_EDITOR
[CustomEditor(typeof(WorldSpawner))]
public class WorldSpanwerGUI : Editor
{
    public override void OnInspectorGUI () 
    {
        DrawDefaultInspector();

        WorldSpawner script = target as WorldSpawner;

        if (GUILayout.Button("Generate World"))
        {
            script.StartCoroutine("BuildWorld");
        }
        if (GUILayout.Button("Destory World"))
            script.DeleteWorld();
    }
}
#endif
