using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public enum SelectWall
//{
//    select, // 선택후 돌아오는 항목
//    none,   // 이미 있는 wall 삭제
//    wall1,
//    wall2,
//    wall3,
//    wall4
//}
public class WallSelector : MonoBehaviour
{
    //public SelectWall selectedWall = SelectWall.select;
    //private WallInfo[] wallInfo;
    private GameObject currentWall;


    public void SetWall(int WallId) {
        currentWall = Instantiate(WallDatabase.Instance.GetByID(WallId).wallPrefab, transform.position + Vector3.up * .5f, Quaternion.identity ,transform.parent);
     }
}