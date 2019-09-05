using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class MaterialMapper : EditorWindow
{

    #region  Variables
    public Texture2D logo;

    public List<Object> characters = new List<Object> { null };

    public ModelImporterMaterialSearch searchType;
    #endregion
    #region Window Init
    [MenuItem ("Custom/Material Mapper")]
    public static void ShowWindow () {        
        EditorWindow.GetWindow(typeof(MaterialMapper));
    }
    #endregion
    #region LoadAllAssetsFromPath
    public static T[] GetAtPath<T>(string path)
    {

        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);       



        foreach (string fileName in fileEntries)
        { 
            int assetPathIndex = fileName.IndexOf("Assets");
            string localPath = fileName.Substring(assetPathIndex);
            
            Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

            if (t != null)
                al.Add(t);
        }
        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];

        return result;
    }
    #endregion
    #region WindowGUI
    void OnGUI () {

        EventType e = Event.current.type;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+ (Add)"))
        {
            characters.Add(null);
        }
        if (GUILayout.Button("- (Remove)"))
        {
            characters.RemoveAt(characters.Count - 1);
        }
        GUILayout.EndHorizontal();

        if (logo == null)
            logo = (Texture2D)Resources.Load("MaterialMapper", typeof(Texture2D));

        #region Logo
        // Editor Icon
            GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(logo, GUILayout.MaxHeight(135), GUILayout.Width(position.width));
                GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        #endregion       
        
        // List Logic
        if (characters.Count > 0)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            for (int i = 0; i < characters.Count; i++)
            {
                characters[i] = EditorGUILayout.ObjectField(characters[i], typeof(Object), true) as Object;
            }
            GUILayout.EndVertical();
        }
        
        EditorGUILayout.Space();

        // Search type GUI
        searchType = (ModelImporterMaterialSearch)EditorGUILayout.EnumPopup("Search Type: ", searchType);

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();        
            // Begin assinging the materials
            if (GUILayout.Button("Assign Materials"))
            {
                MatSearcher map = new MatSearcher();

                map.RemapAssetMaterials(characters, searchType);

                map = null;
            }
            // Fix The Animation names to match the file names
            if (GUILayout.Button("Fix Animation Names"))
            {
                MatSearcher map = new MatSearcher();

                map.FixAnimationsNames(characters);

                map = null;
            }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (GUILayout.Button("Clear Selection"))
        {
            if (characters.Count > 0)
            {
                characters = new List<Object>
                {
                    null
                };
            }
        }
        if (GUILayout.Button("Load Assets from Folder"))
        {
            if (characters[0] != null)
            {
                string holder = AssetDatabase.GetAssetPath(characters[0]);
                string holder2 = holder.Substring(7, holder.Length - 7);

                int slashIndex = 0;
                int iterator = 0;

                // Find the last Slash to remove from the search index
                while (iterator < holder2.Length)
                {
                    if (holder2[iterator] == '/')
                        slashIndex = iterator;

                    iterator++;
                }

                string holder3 = holder2.Substring(0, slashIndex + 1);


                Object[] temp = GetAtPath<Object>(holder3);

                if (temp.Length > 0)
                {
                    characters.Clear();
                    characters = temp.OfType<Object>().ToList();
                }
            }
        }
    }
    #endregion
}

// Material Class for doing the heavy lifting, derives from a different class.
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

    public void FixAnimationsNames(List<Object> subAsset)
    {
        int iterator = 0;
        if (subAsset.Count > 0)
        {
            foreach (Object path in subAsset)
            {
                string assetPath = AssetDatabase.GetAssetPath(path);
                ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                var defaultClipAnimations = modelImporter.defaultClipAnimations;
                foreach (var clipAnimation in defaultClipAnimations)
                {
                    clipAnimation.name = subAsset[iterator].name;
                    clipAnimation.loop = true;
                    clipAnimation.loopTime = true;
                    clipAnimation.loopPose = true;
                }
                modelImporter.clipAnimations = defaultClipAnimations;
                modelImporter.SaveAndReimport();
                iterator++;
            }
        }
    }
}