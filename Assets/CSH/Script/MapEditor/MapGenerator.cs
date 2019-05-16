using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

    public GameObject outWallPrefab;
	public Transform tilePrefab;
	public Vector2 mapSize;

	[Range(0,1)]
	public float outlinePercent;

	void Start() {
		//GenerateMap ();
	}

	public void GenerateMap() {

		string holderName = "Generated Map";
		if (transform.Find (holderName)) {
			DestroyImmediate(transform.Find(holderName).gameObject);
		}

		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;

		for (int x = -1; x < mapSize.x+1; x ++) {
			for (int y = -1; y < mapSize.y+1; y ++) {
                Vector3 tilePosition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);

                if (x == -1 || x == mapSize.x || y == -1 || y == mapSize.y) {
                    GameObject newTile = Instantiate(outWallPrefab, tilePosition + Vector3.up * .5f, Quaternion.identity) as GameObject;
                    newTile.transform.localScale = Vector3.one * (1 - outlinePercent);
                    newTile.transform.parent = mapHolder;
                }
                else {
                    
                    Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                    newTile.localScale = Vector3.one * (1 - outlinePercent);
                    newTile.parent = mapHolder;

                }
            }
		}
	}
}
