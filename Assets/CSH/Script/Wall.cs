/*
 * Wall.cs
 * 
 * 요약
 * Wall 속성을 갖는 Cube에 포함되어 Grid에 현재 위치에 Wall이 있다고 알려준다.
 * 
 * 수정
 * BlockSystem.cs에 적어놓은 대대적인 수정사항 참고
 * 
 */

using UnityEngine;
using System.Collections;

//wall과 block을 아우를 수 있는 blockObject 클래스를 선언 후 각 클래스가 상속받도록 하면 유지보수에 좋을 것 같다
public class Wall : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;      //Store a component reference to the attached SpriteRenderer.
    public TextMesh tm;
    ClientLibrary.Grid grid;
    void Awake() {
        //Get a component reference to the SpriteRenderer.
        spriteRenderer = GetComponent<SpriteRenderer>();

        grid = ClientLibrary.Grid.instance;

        
    }

    private void Start()
    {

        ClientLibrary.Grid.instance.GetSetBoolWallPosition(transform.position, true);

        //tm.text = ClientLibrary.Grid.instance.GridPositionToArray(transform.position).x + ", " + ClientLibrary.Grid.instance.GridPositionToArray(transform.position).z + "\n" + ClientLibrary.Grid.instance.BoolCurrentPosition(transform.position);
    }



}
