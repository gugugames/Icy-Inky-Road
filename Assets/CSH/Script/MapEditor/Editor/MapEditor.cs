using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class MapEditor : Editor {

	public override void OnInspectorGUI ()
	{
        DrawDefaultInspector();

        if (GUILayout.Button("Refresh")) {
            base.OnInspectorGUI();
            MapGenerator map = target as MapGenerator;
            map.GenerateMap ();
        }
	}
	
}
