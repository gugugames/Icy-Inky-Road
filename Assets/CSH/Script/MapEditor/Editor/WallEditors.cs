using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(WallSelector))]
public class WallEditors : Editor {


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //base.OnInspectorGUI();

        WallSelector wall = target as WallSelector;

        GUILayout.Label("Wall Select");
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Wall1")) {
            wall.SetWall(0);
        }
        if (GUILayout.Button("Wall2")) {
            wall.SetWall(1);
        }
        if (GUILayout.Button("Wall3")) {
            wall.SetWall(1);
        }

        GUILayout.EndHorizontal();

        //if (wall.selectedWall != SelectWall.select)
        //{
        //    wall.SetWall();
        //    wall.selectedWall = SelectWall.select;
        //}

    }

}
