using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TileWorld : MonoBehaviour
{
    public float XOffset = 1, YOffset = 1;
    public int amtX, amtY;
    public GameObject tile;
    public LayerMask lm;
    public void Build()
    {
        StartCoroutine(DestroyTiles(done => {

            if (done)
            {
                float X = transform.position.x;
                float Y = transform.position.y;
                int counter = 0;
                for (int i = 0; i < amtX; i++)
                {
                    for (int j = 0; j < amtY; j++)
                    {
                        GameObject Tile = Instantiate(tile, new Vector3((XOffset * i), transform.position.y, (YOffset * j)), Quaternion.identity);
                        Tile.name = "Tile: " + counter;
                        Tile.transform.SetParent(transform);
                        counter++;                        
                    }
                }
            }            
        }));
    }

    public void ValidateTiles()
    {            
        List<GameObject> tiles = new List<GameObject>();
        foreach (Transform child in gameObject.transform) {
            tiles.Add(child.gameObject);
        }

        for (int i = tiles.Count-1; i > 0; i--)
        {        
            if (tiles[i] != null)            
            {
                Collider[] hitColliders = Physics.OverlapSphere(tiles[i].transform.position, (XOffset + YOffset)/2.5f);
                if (hitColliders.Length > 0)
                {
                    for (int k = 0; k < hitColliders.Length; k++)
                    {
                        try
                        {
                            if (hitColliders[k] != null && hitColliders[k].tag != "MoveableLocations" && hitColliders[k].tag != "Ground")
                                DestroyImmediate(tiles[i]);
                        }
                        catch (System.Exception e)
                        {
                            print(e);
                            throw;
                        }

                    }
                }
            }    
        }
    }

    IEnumerator DestroyTiles(System.Action<bool> done)
    {
        while (gameObject.transform.childCount > 0)
        {
            foreach (Transform child in gameObject.transform) {
                DestroyImmediate(child.gameObject);
            }
        }

        done(true);  
        yield return null;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(TileWorld))]
public class TileEditor : Editor
{
     public override void OnInspectorGUI()
    {        
        DrawDefaultInspector ();
        TileWorld script = (TileWorld)target;

        if (GUILayout.Button("Rebuild Tile Set"))
            script.Build();
        if (GUILayout.Button("Validate Map"))
            script.ValidateTiles();
    }
}
#endif