/*
 * Grid.cs
 * 
 * 요악
 * Grid를 생성, World Space에서의 위치를 2차원 Grid 내부의 index로 변환하는 등의 역할을 한다.
 * 
 * 수정
 * BoolCurrentPosition 보다는 IsOccupied 또는 Exist로 매서드명을 바꾸는게 낫지 않을까. 
 * 단순히 Grid에 현재 무언가가 있는지 없는지를 판단하는 걸 넘어서 무엇이 있는지를 확실하게 알 수 있도록 GameObject 또는 Transform의 이차원 배열을 이용하는 것도 좋을듯. 
 * 현재 구현은 [mapSize, mapSize] 크기의 2차원 배열을 활용하므로 정사각형 맵만 지원한다. MapGenerator.cs에서 가로 세로의 길이가 다르게 지정할 수 있는 것과 다르다. 
 * 
 */

using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;
using Photon.Pun;

namespace ClientLibrary
{
    public class Grid : MonoBehaviourPun
    {
        [SerializeField]
        private int size = 1;
        public static Grid instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        public int mapSize = 12;
        public bool[,] mapArray;  //bool값의 변수명은 isInMap, isOccupied 등 yes/no로 표현할 수 있는 게 좋을듯
        public bool[,] playerOccupationArray;
        public string[,] shareArray; //share이라는 의미가 조금 불분명한 것 같아서 점유팀array라는 의미를 담을 수 있도록 변수명을 변경하는게 좋을듯

        private int sharePointA = 0;
        private int sharePointB = 0;

        ExitGames.Client.Photon.Hashtable PlayerCustomProps = new ExitGames.Client.Photon.Hashtable();

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

                //player 점유 위치
                InitPlayerOccupationArray();

                //if not, set instance to this
                instance = this;
            }

                

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            //DontDestroyOnLoad(gameObject);
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

        //map Size 초기화
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
            print("InitMapArray done");
        }

        public void InitPlayerOccupationArray()
        {
            playerOccupationArray = new bool[mapSize, mapSize];
            for (int x = 0; x < mapSize; x += size)
            {
                for (int z = 0; z > mapSize; z += size)
                {
                    playerOccupationArray[x, z] = false;
                }

            }
            print("InitMapArray done");
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
            print("InitMapShareArray done");
        }

        //Map 점유율
        //PlayerCtrl의 PlayerTeam enum 값으로 처리함
        public void SetMapShareArray(Vector3 position, PlayerCtrl.PlayerTeam playerTeam)
        {
            Vector3 gridPosition = GridPositionToArray(position);
            if (shareArray[(int)gridPosition.x, (int)gridPosition.z] == playerTeam.ToString())
            {
                //return;
            }
            else
            {
                shareArray[(int)gridPosition.x, (int)gridPosition.z] = playerTeam.ToString();
                print("set map share array : " + shareArray[(int)gridPosition.x, (int)gridPosition.z]);

            }
        }

        //매개변수 position의 점유자 반환
        public string GetMapShareArray(Vector3 position)
        {
            Vector3 gridPosition = GridPositionToArray(position);
            return shareArray[(int)gridPosition.x, (int)gridPosition.z];
        }

        //매개변수 position의 값을 array index 값으로 변환하여 반환
        public Vector3 GridPositionToArray(Vector3 position)
        {
            return new Vector3(position.x + (mapSize / 2) - 0.5f, 
                position.y,
                position.z + (mapSize / 2) - 0.5f);
        }

         // 매개변수 x,y(array index) 값을 position 값으로 변환하여 반환
        public Vector2 GridArrayToPosition(int x, int y)
        {
            return new Vector2((-mapSize / 2 - 0.5f) + x, (-mapSize / 2 - 0.5f) + y);
        }

         //매개변수 position의 값을 grid 값으로 변환하여 반환
        public Vector3 GetCurrentGrid(Vector3 position)
        {
            return GetNearestPointOnGrid(position);
        }

        //현재 위치의 바로 후 grid 반환
        public Vector3 GetNextGrid(Vector3 position, Vector3 dir)
        {
            return GetNearestPointOnGrid(position + dir);
        }

        //현재 위치의 바로 직전 grid 반환
        public Vector3 GetPreviousGrid(Vector3 position, Vector3 dir)
        {
            return GetNearestPointOnGrid(position - dir);
        }

        // 매개변수 position - 값을 grid로 변환하여 벽의 유무 리턴
        // 매개변수 setBool - !null : setBool 값으로 position의 mapArray 값을 세팅함 (true : 벽존재 / false : 벽 없음)
        // 매개변수 setBool - null : position값으로 mapArray 값 반환
        public bool GetSetBoolWallPosition(Vector3 position, bool? setBool = null) 
        //메소드명에 get과 set이 함께 있는게 굉장히 어색해보여서 메소드명에 대한 논의가 필요해보임. isInMapArray 와 같은 표현은 어떨지..                                 
        {
            Vector3 gridPosition = GridPositionToArray(position);
            if (setBool != null)
                mapArray[(int)gridPosition.x, (int)gridPosition.z] = setBool.Value;
            //print("GridPositionToArray : " + (int)gridPosition.x + " , " + (int)gridPosition.z);
            return mapArray[(int)gridPosition.x, (int)gridPosition.z];
        }

        //
        public bool GetSetBoolPlayerOcuupationPosition(Vector3 position, PlayerCtrl.PlayerTeam playerTeam = PlayerCtrl.PlayerTeam.empty, bool? setBool = null)
        {
            Vector3 gridPosition = GridPositionToArray(position);
            if (setBool != null)
            {
                //점수 계산부분
                if (PhotonNetwork.IsMasterClient && playerOccupationArray[(int)gridPosition.x, (int)gridPosition.z] == false && setBool == true)
                    CalculateShare(position, playerTeam);
                playerOccupationArray[(int)gridPosition.x, (int)gridPosition.z] = setBool.Value;
            }
            //print("GridPositionToArray : " + (int)gridPosition.x + " , " + (int)gridPosition.z);
            return playerOccupationArray[(int)gridPosition.x, (int)gridPosition.z];
        }

        //입력된 position위에 있는 object가 wall 인지 아닌지 체크
        public bool CheckWallPosition(Vector3 position)
        {
            Vector2 temp1 = GridArrayToPosition(0, 0);
            Vector2 temp2 = GridArrayToPosition(mapSize+1, mapSize+1);
            //print("temp1 : " + temp1 + "position : " + position);
            if (position.x <= temp1.x|| position.x == temp2.x 
                || position.y == temp1.y || position.y == temp2.y)
            {
                return true;
            }
            return false;
        }

        //map에 object가 있는지 bool 값으로 체크 (true: object 존재, false object 없음)
        public string CalculateShare(Vector3 position, PlayerCtrl.PlayerTeam playerTeam = PlayerCtrl.PlayerTeam.empty)
        //함수 이름은 동사+명사 형식이 적합할듯 하다! ex) CalculatePoint
        //한 함수 안에서 두가지 일을 동시에 하고 있다(현재 점유팀의 이름 반환과 영역 계산) -> 두함수로 쪼개서 처리하는게 좋을 것 같다
        {
            Vector3 gridPosition = GridPositionToArray(position);
            string occupation = shareArray[(int)gridPosition.x, (int)gridPosition.z]; //currentOccupationTeam?

            if (playerTeam == PlayerCtrl.PlayerTeam.empty)
            {
                return shareArray[(int)gridPosition.x, (int)gridPosition.z];
            }

            //점유 되어있는 곳이 없고 B가 점유를 할 예정일때
            if (occupation == null && playerTeam == PlayerCtrl.PlayerTeam.B)
            {
                //B의 점수를 올린다.
                sharePointB++;
                print("B++");
            }
            //점유 되어있는 곳이 없고 A가 점유를 할 예정일때
            else if (occupation == null && playerTeam == PlayerCtrl.PlayerTeam.A)
            {
                //A의 점수를 올린다.
                sharePointA++;
                print("A++");
            }
            //점유 되어있는 곳이 A이고 B가 점유를 할 예정일때
            else if (occupation == "A" && playerTeam == PlayerCtrl.PlayerTeam.B)
            {
                //B의 점수를 올리고 A점수를 낮춘다.
                sharePointA--;
                sharePointB++;
            }
            //점유 되어있는 곳이 B이고 A가 점유를 할 예정일때
            else if (occupation == "B" && playerTeam == PlayerCtrl.PlayerTeam.A)
            {
                //A의 점수를 올리고 B점수를 낮춘다.
                sharePointA++;
                sharePointB--;
            }

            ScoreCustomProps();

            return shareArray[(int)gridPosition.x, (int)gridPosition.z];
        }

        public void ScoreCustomProps()
        {
            PlayerCustomProps["ScoreA"] = sharePointA;
            PlayerCustomProps["ScoreB"] = sharePointB;

            PhotonNetwork.MasterClient.SetCustomProperties(PlayerCustomProps);
        }

        public string GetShareText()
        {
            return "A : " + sharePointA + ", B : " + sharePointB;
        }

        public string GetShare(Vector3 position)
        {
            return CalculateShare(position);
        }
    }

}

