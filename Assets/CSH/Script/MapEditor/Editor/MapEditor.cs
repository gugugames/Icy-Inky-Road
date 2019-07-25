/*
 * MapEditor.cs
 * 
 * Unity Editor에서 직접 Map Editor를 사용할 수 있도록 하는 UI 기능을 제공하는 스크립트.
 * Map GameObject의 Inspector에서 Refresh 버튼을 누르면 MapGenerator.cs의 GenerateMap 매서드가 맵을 지정한 대로 재생성한다. 
 * 
 */

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
