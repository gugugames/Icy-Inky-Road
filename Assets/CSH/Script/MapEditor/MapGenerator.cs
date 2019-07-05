/*
 * MapGenerator.cs
 * 
 * Unity Editor 환경에서 직접 동적인 맵 생성 기능을 제공하는 Script.
 * 외벽, 타일, 맵의 크기, 외곽선 너비 등을 지정할 수 있다. 
 * Unity Editor와의 상호작용은 MapEditor.cs에서 관리하며 Refresh 버튼을 눌렀을 때 GenerateMap() 매서드가 호출된다.
 * 
 */

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
