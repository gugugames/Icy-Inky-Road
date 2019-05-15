using UnityEngine;
using System;
using UnityEditor;

namespace ClientLibrary
{
    public class Grid : MonoBehaviour
    {
        [SerializeField]
        private int size = 1;
        public static Grid instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        public int mapSize = 10;
        public bool[,] mapArray;
        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)
            {
                SetMapArray();

                //if not, set instance to this
                instance = this;
            }

                

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }


        public Vector3 GetNearestPointOnGrid(Vector3 position) {
            position -= transform.position;

            float xCount = Mathf.RoundToInt(position.x / size);
            float yCount = Mathf.RoundToInt(position.y / size);
            float zCount = Mathf.RoundToInt(position.z / size);

            Vector3 result = new Vector3(
                (float)xCount * size - 0.5f ,
                (float)yCount * size + 0.5f,
                (float)zCount * size - 0.5f );

            result += transform.position;

            return result;
        }

        public float GetGridDistance()
        {
            return GetNearestPointOnGrid(new Vector3(1, 0f, 0)).x - GetNearestPointOnGrid(new Vector3(2, 0f, 0)).x;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            for (float x = -mapSize/2 + 1; x < mapSize/2 + 1; x += size) {
                for (float z = mapSize / 2; z > -mapSize / 2; z -= size) {
                    var point = GetNearestPointOnGrid(new Vector3(x, 0f, z));
                    Gizmos.DrawSphere(point, 0.1f);
                }

            }
        }

        public void SetMapArray()
        {
            mapArray = new bool[mapSize,mapSize];
            for (int x = 0; x < mapSize; x += size)
            {
                for (int z = 0; z > mapSize; z += size)
                {
                    mapArray[x,z] = false;
                }

            }
        }

        public Vector3 GridPositionToArray(Vector3 position)
        {
            print("AAAA : " + (position.x + (mapSize / 2) - 0.5f));
            return new Vector3(position.x + (mapSize / 2) - 0.5f, 
                position.y,
                position.z + (mapSize / 2) - 0.5f);
        }

        public bool BoolCurrentPosition(Vector3 position, bool? setBool = null)
        {
            Vector3 gridPosition = GridPositionToArray(position);
            if(setBool != null)
                mapArray[(int)gridPosition.x, (int)gridPosition.z] = setBool.Value;
            print("GridPositionToArray : " + mapArray[0,0]);
            return mapArray[(int)gridPosition.x, (int)gridPosition.z];
        }
    }

}

