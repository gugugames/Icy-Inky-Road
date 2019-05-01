using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSelector : MonoBehaviour {

    public enum SelectWall
    {
        none,
        wall1,
        wall2,
        wall3,
        wall4
    }

    public SelectWall selectedWall = SelectWall.none;
    private WallInfo[] wallInfo;
    private GameObject currentWall;

    public void SetWall() {
        switch (selectedWall) {
            case SelectWall.none:
                if(currentWall != null)
                    Destroy(currentWall);
                break;
            case SelectWall.wall1:
                if (currentWall != null)
                    Destroy(currentWall);
                else
                    currentWall = Instantiate(wallInfo[0].wallGameObject);
                break;
            case SelectWall.wall2:
                if (currentWall != null)
                    Destroy(currentWall);
                else
                    currentWall = Instantiate(wallInfo[1].wallGameObject);
                break;
            case SelectWall.wall3:
                if (currentWall != null)
                    Destroy(currentWall);
                else
                    currentWall = Instantiate(wallInfo[2].wallGameObject);
                break;
            case SelectWall.wall4:
                if (currentWall != null)
                    Destroy(currentWall);
                else
                    currentWall = Instantiate(wallInfo[3].wallGameObject);
                break;
            default:
                break;
        }
    }




}
