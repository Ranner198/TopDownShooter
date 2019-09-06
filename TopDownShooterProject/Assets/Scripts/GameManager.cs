using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public int numOfFriendlies;
    public GameObject friendlyPrefab;
    public GameObject SpawnPoint;
    public List<PlayerManager> friendlies = new List<PlayerManager>();
    public List<GameObject> enemies = new List<GameObject>();
    public Camera cam;
    public LayerMask lm;
    public float spawnTimer;
    public NightVision nightVision;
    public bool nightVisionEnabled;
    public static GameManager instance;    
    public bool special;
    public enum SpecialType {SmokeGernade};
    public SpecialType specialType;
    public GameObject SmokeGernade;
    public Texture2D position;
    public enum GameMode {Seize, Eliminate, Hostage}
    public GameMode gameMode;

    // Game mode specific
    public GameObject Objective;
    public bool MissionComplete;

    public void Awake()
    {
        instance = this;

        //Cursor.SetCursor(position, Vector2.zero, CursorMode.Auto);
    }

    public void SpawnFriendlies()
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
    public void PopEnenmy(GameObject go)
    {
        enemies.Remove(go);

        // Come up with a way to do Evac here
        if (go == Objective)
        {
            MissionComplete = true;
            print("Mission Complete Solider");
        }
        else
        {
            print("Not it chief");
        }
            
    }
    public void SpawnSmokeGernade()
    {
        special = true;
        specialType = SpecialType.SmokeGernade;
    }

    // Remove when we swap over to spawning
    public void Start()
    {
        gameMode = GameMode.Eliminate;
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemies.Add(enemy);
        }

        int rand = Random.Range(0, enemies.Count-1);

        Objective = enemies[rand];
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
                        if (!special)
                        {
                            foreach(PlayerManager player in friendlies)
                            {
                                player.MoveTo(hit.point);
                            }
                        }
                        else
                        {
                            switch (specialType)
                            {
                                case SpecialType.SmokeGernade:

                                    special = false;
                                    Vector3 pos = hit.point;
                                    pos.y = 25;
                                    GameObject smoke = Instantiate(SmokeGernade, pos, Quaternion.identity);
                                    smoke.transform.GetChild(1).GetComponent<SmokeScreen>().Target = hit.point;

                                    int rand = Random.Range(0, friendlies.Count-1);
                                    
                                    friendlies[rand].Throw(hit.point, smoke);

                                    smoke.name = "Smoke Gernade";

                                break;
                            }
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

        if (Input.GetKeyDown(KeyCode.N))
        {
            nightVisionEnabled = !nightVisionEnabled;
            nightVision.enabled = nightVisionEnabled;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (PlayerManager player in friendlies)
            {
                player.state = PlayerManager.State.Reloading;
                player.UpdateAnimationController();
            }
        }
    }
}
