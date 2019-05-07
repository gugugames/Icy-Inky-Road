using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(WallSelector))]
public class WallEditors : Editor {

    int selected;



    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //base.OnInspectorGUI();
        GUILayout.Label("Wall Select");
        selected = GUILayout.Toolbar(selected, new string[] { "제거", "2", "3" });

        WallSelector wall = target as WallSelector;

        //현재 Id와 이전 Id를 비교해서 변경 되었을 때만 실행
        if (selected != wall.currentId) {
            wall.SetWall(selected);
            wall.currentId = selected;
        }
    }
}

//나중에 언젠간 쓸일이 있을거같아서 남겨놓는 쓰레기값
#region

//switch (selected) {
//    case 0:
//        break;
//}
//GUILayout.BeginHorizontal();

//if (GUILayout.Button("Wall1")) {
//    wall.SetWall(0);
//}
//if (GUILayout.Button("Wall2")) {
//    wall.SetWall(1);
//}
//if (GUILayout.Button("Wall3")) {
//    wall.SetWall(1);
//}

//GUILayout.EndHorizontal();

//if (wall.selectedWall != SelectWall.select)
//{
//    wall.SetWall();
//    wall.selectedWall = SelectWall.select;
//}
#endregion
