using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int numOfFriendlies;
    public GameObject friendlyPrefab;
    public GameObject SpawnPoint;
    private List<PlayerManager> friendlies = new List<PlayerManager>();
    public Camera cam;
    public LayerMask lm;

    public void Start()
    {
        for (int i = 0; i < numOfFriendlies; i++)
        {
            Vector3 SpawnPointPos = SpawnPoint.transform.position;
            // Randomize the positon
            SpawnPointPos.x += Random.Range(-2, 2);
            SpawnPointPos.z += Random.Range(-2, 2);

            GameObject temp = Instantiate(friendlyPrefab, SpawnPointPos, Quaternion.identity);
            temp.name = "Friendly: " + i;

            friendlies.Add(temp.GetComponent<PlayerManager>());            
            friendlies[i].index = i;
        }
    }

    public void Update()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, lm))
        {             
            switch (hit.transform.tag)
            {
                case "Ground":
                    if (Input.GetMouseButtonDown(1))
                    {
                        foreach(PlayerManager player in friendlies)
                        {
                            player.MoveTo(hit.point);
                        }
                    }
                break;
                case "Room":
                    if (Input.GetMouseButtonDown(1))
                    {
                        foreach(PlayerManager player in friendlies)
                        {
                            player.MoveTo(hit.point);
                        }
                    }
                break;
                case "Enemy":
                    if (Input.GetMouseButtonDown(1))
                    {
                        foreach(PlayerManager player in friendlies)
                        {
                            player.Attack(hit.transform.gameObject);
                        }
                    }
                break;
            }
        }
    }
}
