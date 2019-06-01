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
        public int mapSize = 12;
        public bool[,] mapArray;
        public string[,] shareArray;

        private int sharePointA = 0;
        private int sharePointB = 0;

        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)
            {
                //position array
                InitMapArray();

                //grid 점유율
                InitMapShareArray();

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

        
        public void InitMapArray()
        {
            mapArray = new bool[mapSize,mapSize];
            for (int x = 0; x < mapSize; x += size)
            {
                for (int z = 0; z > mapSize; z += size)
                {
                    mapArray[x,z] = false;
                }

            }
            print("setmap done");
        }

        //Map 점유율 초기화
        public void InitMapShareArray()
        {
            shareArray = new string[mapSize, mapSize];
            for (int x = 0; x < mapSize; x += size)
            {
                for (int z = 0; z > mapSize; z += size)
                {
                    shareArray[x, z] = null;
                }

            }
            print("setmap done");
        }

        //Map 점유율
        //PlayerCtrl의 PlayerTeam enum 값으로 처리함
        public void SetMapShareArray(Vector3 position, PlayerCtrl.PlayerTeam playerTeam)
        {
            Vector3 gridPosition = GridPositionToArray(position);
            shareArray[(int)gridPosition.x, (int)gridPosition.z] = playerTeam.ToString();
            print("set map share array : " + shareArray[(int)gridPosition.x, (int)gridPosition.z]);
        }

        public Vector3 GridPositionToArray(Vector3 position)
        {
            //print("AAAA : " + (position.x + (mapSize / 2) - 0.5f));
            return new Vector3(position.x + (mapSize / 2) - 0.5f, 
                position.y,
                position.z + (mapSize / 2) - 0.5f);
        }

        public Vector3 GetCurrentGrid(Vector3 position, Vector3? dir = null)
        {
            //print("GetCurrnetGrid : " + grid.GetNearestPointOnGrid(transform.position));
            return GetNearestPointOnGrid(position);
        }

        public Vector3 GetNextGrid(Vector3 position, Vector3 dir)
        {
            //print("GetCurrnetGrid : " + GetNearestPointOnGrid(transform.position + dir * 2));
            return GetNearestPointOnGrid(position + dir);
        }

        //나중에 두 메서드 합쳐야함 BoolCurrentPosition, GridShare
        public bool GetSetBoolCurrentPosition(Vector3 position, bool? setBool = null)
        {
            //print("AA");
            Vector3 gridPosition = GridPositionToArray(position);
            if (setBool != null)
                mapArray[(int)gridPosition.x, (int)gridPosition.z] = setBool.Value;
            //print("GridPositionToArray : " + (int)gridPosition.x + " , " + (int)gridPosition.z);
            return mapArray[(int)gridPosition.x, (int)gridPosition.z];
        }

        //map에 object가 있는지 bool 값으로 체크 (true: object 존재, false object 없음)
        public string SetGridShare(Vector3 position, string shareTeam = null)
        {
            //print("AA");
            Vector3 gridPosition = GridPositionToArray(position);


            //점유 되어있는 곳이 없고 B가 점유를 할 예정일때
            if (SetGridShare(position) == null && shareTeam == "B")
            {
                //B의 점수를 올린다.
                sharePointB++;
            }
            //점유 되어있는 곳이 없고 A가 점유를 할 예정일때
            else if (SetGridShare(position) == null && shareTeam == "A")
            {
                //A의 점수를 올린다.
                sharePointA++;
            }
            //점유 되어있는 곳이 A이고 B가 점유를 할 예정일때
            else if (SetGridShare(position) == "A" && shareTeam == "B")
            {
                //B의 점수를 올리고 A점수를 낮춘다.
                sharePointA--;
                sharePointB++;
            }
            //점유 되어있는 곳이 B이고 A가 점유를 할 예정일때
            else if (SetGridShare(position) == "B" && shareTeam == "A")
            {
                //A의 점수를 올리고 B점수를 낮춘다.
                sharePointA++;
                sharePointB--;
            }
            

            if (shareTeam != null)
            {
                shareArray[(int)gridPosition.x, (int)gridPosition.z] = shareTeam;
                print(GetShareText() + "");
            }
            //print("GridPositionToArray : " + (int)gridPosition.x + " , " + (int)gridPosition.z);

            return shareArray[(int)gridPosition.x, (int)gridPosition.z];
        }
        
        public string GetShareText()
        {
            return "A : " + sharePointA + ", B : " + sharePointB;
        }

        public string GetShare(Vector3 position)
        {
            return SetGridShare(position);
        }

        public void SetShare(bool whoGetPoint)
        {
            if(whoGetPoint == true)
            {
                sharePointA++;
            }
        }


    }

}

