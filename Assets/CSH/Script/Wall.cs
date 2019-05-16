using UnityEngine;
using System.Collections;


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

        ClientLibrary.Grid.instance.BoolCurrentPosition(transform.position, true);

        tm.text = ClientLibrary.Grid.instance.GridPositionToArray(transform.position).x + ", " + ClientLibrary.Grid.instance.GridPositionToArray(transform.position).z + "\n" + ClientLibrary.Grid.instance.BoolCurrentPosition(transform.position);
    }



}
