using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public int numOfFriendlies, numOfEnemies;
    public GameObject friendlyPrefab, enemyPrefab;
    public GameObject SpawnPoint;
    public List<PlayerManager> friendlies = new List<PlayerManager>();
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> buildings = new List<GameObject>();
    public Camera cam;
    public LayerMask lm;
    public float spawnTimer;
    public NightVision nightVision;
    public bool nightVisionEnabled;
    public static GameManager instance;    
    public bool special;
    public enum SpecialType {SmokeGernade, Gernade};
    public SpecialType specialType;
    public GameObject SmokeGernade, Gernade;
    public Texture2D position;
    public enum GameMode {Seize, Eliminate, Hostage}
    public GameMode gameMode;    

    // Game mode specific
    public GameObject Objective;
    public bool MissionComplete;

    // Canvas Elements
    public GameObject playerCardParent;
    public GameObject playerCard;

    public void Awake()
    {
        instance = this;

        //Cursor.SetCursor(position, Vector2.zero, CursorMode.Auto);
    }

    public void Start()
    {
        SpawnEnemies();

        gameMode = GameMode.Eliminate;

        int rand = Random.Range(0, enemies.Count-1);

        Objective = enemies[rand];

        for (int i = 0; i < numOfFriendlies; i++)
        {
            GameObject playerCardTemp = GameObject.Instantiate(playerCard, Vector3.zero, Quaternion.identity);
            
            playerCardTemp.name = "PlayerCard: " + i;
            playerCardTemp.transform.SetParent(playerCardParent.transform);
        }
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

    public void SpawnEnemies()
    {
        for (int i = 0; i < numOfEnemies; i++)
        {
            Vector3 SpawnPointPos = buildings[Random.Range(0, buildings.Count-1)].transform.position;
            // Randomize the positon
            SpawnPointPos.x += Random.Range(-2, 2);
            SpawnPointPos.z += Random.Range(-2, 2);

            GameObject temp = Instantiate(enemyPrefab, SpawnPointPos, Quaternion.identity);
            temp.name = "Enenmy: " + i;

            enemies.Add(temp);                        
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
    public void SpawnGernade()
    {
        special = true;
        specialType = SpecialType.Gernade;
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
                            if (Random.Range(0f, 1f) > .8f)
                                AudioManger.instance.Play("MovingOntoObjective");

                            foreach(PlayerManager player in friendlies)
                            {
                                player.MoveTo(hit.point);
                            }
                        }
                        else
                        {
                            // Preallocae the data needed
                            special = false;
                            Vector3 pos = hit.point;
                            int rand = Random.Range(0, friendlies.Count-1);

                            switch (specialType)
                            {
                                case SpecialType.SmokeGernade:
                                    GameObject smoke = Instantiate(SmokeGernade, pos, Quaternion.identity);
                                    smoke.transform.GetChild(1).GetComponent<SmokeScreen>().Target = hit.point;

                                    friendlies[rand].Throw(hit.point, smoke, specialType);

                                    smoke.name = "Smoke Gernade";                                    
        
                                break;
                                case SpecialType.Gernade:

                                    GameObject gernade = Instantiate(Gernade, pos, Quaternion.identity);
                                    gernade.GetComponent<Gernade>().Target = hit.point;
 
                                    friendlies[rand].Throw(hit.point, gernade, specialType);

                                    gernade.name = "Gernade";                                    
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
        if (Input.GetKeyDown(KeyCode.S))
            SpawnSmokeGernade();
        if (Input.GetKeyDown(KeyCode.G))
            SpawnGernade();                    
    }

    public void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "Enenmy")
        {
            coll.gameObject.GetComponent<AudioSource>().volume = .5f;
        }
    }
    
    public void OnTriggerExit(Collider coll)
    {
        if(coll.gameObject.tag == "Enenmy")
        {
            coll.gameObject.GetComponent<AudioSource>().volume = 0;
        }
    }
}
