using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class WallDatabaseEditor {


    private static string GetSavePath()
    {
        return EditorUtility.SaveFilePanelInProject("New wall database", "New wall database", "asset", "Create a new wall database.");
    }

    [MenuItem("Assets/Create/Databases/wall Database")]
    public static void CreateDatabase()
    {
        string assetPath = GetSavePath();
        WallDatabase asset = ScriptableObject.CreateInstance("WallDatabase") as WallDatabase;  //scriptable object
        AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
        AssetDatabase.Refresh();
    }
}
