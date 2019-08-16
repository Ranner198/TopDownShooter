using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AutoAssignMaterials : EditorWindow
{    

    #region  Variables

    public List<Object> characters = new List<Object>();
    public ModelImporterMaterialSearch searchType;

    #endregion


    [MenuItem ("Custom/AssignMaterials")]
    public static void ShowWindow () {        
        EditorWindow.GetWindow(typeof(AutoAssignMaterials));
    }
    
    void OnGUI () { 
    
        // List Logic
        if (characters.Count > 0)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                characters[i] = EditorGUILayout.ObjectField(characters[i], typeof(Object), true) as Object;
            }
        }

        // Add and remove objects from the list
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+"))
        {
            characters.Add(null);
        }
        if (GUILayout.Button("-"))
        {
            characters.RemoveAt(characters.Count-1);
        }
        GUILayout.EndHorizontal();

        // Search type GUI
        searchType = (ModelImporterMaterialSearch)EditorGUILayout.EnumPopup("Search Type: ", searchType);

        // Begin assinging the materials
        if (GUILayout.Button("Assign Materials"))
        {
            MatSearcher map = new MatSearcher();

            map.RemapAssetMaterials(characters, searchType);

            map = null;
        }
    }
}

public class MatSearcher : AssetPostprocessor
{
    public void RemapAssetMaterials(List<Object> subAsset, ModelImporterMaterialSearch searchType)
    {
        // Validate that there is at least 1 Object
        if (subAsset.Count > 0)
        {            
            // Foreach Object in the selected menu
            foreach (Object path in subAsset)
            {
                if (path != null)
                {
                    // Load the asset
                    string assetPath = AssetDatabase.GetAssetPath(path);
                    var assetImporter = AssetImporter.GetAtPath(assetPath);
                    ModelImporter modelImporter = assetImporter as ModelImporter;

                    // Search for the materials and remap them accordingly
                    modelImporter.materialLocation = ModelImporterMaterialLocation.InPrefab;
                    modelImporter.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName, searchType);    

                    assetImporter.SaveAndReimport();                
                }
            }
        }
        else
            Debug.Log("Error, the number of models must be greater than one.");
    }
}
