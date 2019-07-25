/*
 * BuildSystem.cs
 * 
 * 요약
 * 장애물(Block 묶음)의 설치를 담당한다.
 * 드래그 앤 드랍으로 Grid에서 적당한 위치를 찾아 Block을 설치한다.
 * 
 * 수정
 * 하나의 Block을 설치하는 것이 아닌 Block 묶음을 설치할 수 있도록 해야 한다.
 * 장애물 설치는 장애물 선택 -> 장애물 자동 배치 -> 장애물 이동 반복 -> 장애물 최종 설치 순으로 이루어지는데,
 * 장애물의 초기 위치를 지정해 자동 배치하는 매서드가 필요하다. 
 * 또한 e를 눌렀을 때 장애물 이동이 가능한 기존 구현을 장애물을 선택했을 때 이동이 가능하도록 수정해야 한다.
 * BuildingSystem 이름을 BlockManager 혹은 BlockDisposer로 바꾸는게 좋아보인다. BuildingSystem은 Block이라는 개념을 포함하지 않는다. 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

namespace ClientLibrary
{
    public class BuildingSystem : MonoBehaviourPun
    {
        private Grid grid;

        private BlockSystem bSys;

        private Camera playerCamera;

        private bool buildModeOn = false;   //빌딩 모드 켜짐/꺼짐

        private bool canBuild = false;      //true = 빌딩 가능 / false 빌딩 불가능


        [SerializeField]
        private LayerMask buildableSurfacesLayer;

        private Vector3 buildPos;

        private GameObject currentTemplateBlock;//빌딩 모드중 생서된 templateBlock을 저장함

        [SerializeField]
        private GameObject blockTemplatePrefab; //templateBlock
        [SerializeField]
        private GameObject blockPrefab;         //instantiate 될 block

        [SerializeField]
        private Material templateMaterial;      //생성할 템플릿 메테리얼을 저장

        private bool isDrag = false;                    //드래그 중인지 여부 저장 : true = 드래그중 / false = 드래그 해제
        private Vector3 dragPos;                //초기 드래그 위치를 저장
        private Vector3 dragPosition;           //드래그된 위치를 PlaceCubeNear를 거쳐 저장
        private Vector3 initPlayerPosition;     //초기플레이어 위치

        public Button tempButton1;      //Button 빌딩모드 시작/해제 버튼
        public Button tempButton2;      //Button 벽 생성 확정 버튼

        //임시변수 나중에 수정함
        Vector3 temp;

        private void Start() {
            //서버에서 자기 자신을 제외한 다른 플레이어의 접근 제한
            if (!photonView.IsMine)
            {
                return;
            }
            else
            {
                //빌딩모드 시작/해제 버튼
                tempButton1 = GameObject.Find("Canvas/Button").transform.GetComponent<Button>();

                //벽 생성 확정 버튼
                tempButton2 = GameObject.Find("Canvas/Button (1)").transform.GetComponent<Button>();

                //빌딩모드 시작/해제 버튼 메서드 연결
                tempButton1.onClick.AddListener(() => StartBuildingMode());

                //벽 생성 확정 버튼 메서드 연결
                tempButton2.onClick.AddListener(() => PlaceBlock());
                
                grid = FindObjectOfType<Grid>();
                bSys = GetComponent<BlockSystem>();
                playerCamera = FindObjectOfType<Camera>();
            }
        }

        //빌딩모드 시작 메서드
        public void StartBuildingMode()
        {
            //모바일 환경에서 동작하는 부분
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            buildModeOn = !buildModeOn;

                if (buildModeOn)
                {
                    canBuild = true;
                }
                else
                {
                    canBuild = false;
                    Cursor.lockState = CursorLockMode.None;
                }
#endif
                //에디터 환경에서 동작하는 부분
#if UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_STANDALONE || UNITY_WEBPLAYER
            if (Input.GetKeyDown("e"))
            {
                buildModeOn = !buildModeOn;

                if (buildModeOn)
                {
                    canBuild = true;
                }
                else
                {
                    canBuild = false;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
#endif
        }

        

        private void Update() {
            if (!photonView.IsMine)
            {
                return;
            }
            else
            {
                //E버튼 클릭시 빌딩 모드 실행, 다시 E 클릭시 빌딩모드 해제
                StartBuildingMode();

                if (buildModeOn)
                {

                    RaycastHit buildPosHit;

                    if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out buildPosHit, 10, buildableSurfacesLayer))
                    {
                        Vector3 point = buildPosHit.point;
                        temp = point;
                        buildPos = PlaceCubeNear(point);
                        buildPos.y = 0;

                    }
                    else
                    {
                        if (currentTemplateBlock != null)
                            Destroy(currentTemplateBlock.gameObject);

                    }
                }

                if (!buildModeOn && currentTemplateBlock != null)
                {
                    Destroy(currentTemplateBlock.gameObject);
                    canBuild = false;
                }

                //Block 템플릿 실행
                if (buildModeOn && currentTemplateBlock == null)
                {
                    //이부분이 빌딩모드때 보이는 가상 블락 실행부분
                    currentTemplateBlock = Instantiate(blockTemplatePrefab, PlaceCubeNear(buildPos), Quaternion.identity);
                    currentTemplateBlock.GetComponent<MeshRenderer>().material = templateMaterial;
                    print("CC");
                }
                //canBuild

                if (canBuild && currentTemplateBlock != null)
                {
                    //currentTemplateBlock.transform.position = PlaceCubeNear(buildPos);
                    //버튼 눌렀을때 드래그 감지
                    if (Input.GetMouseButtonDown(0))
                    {
                        isDrag = true;
                        dragPos = Input.mousePosition;
                        initPlayerPosition.x = currentTemplateBlock.transform.position.x;
                        initPlayerPosition.z = currentTemplateBlock.transform.position.z;

                        //currentTemplateBlock.transform.position = buildPos;
                        print("AA");
                    }

                    if (Input.GetMouseButton(0))
                    {
                        currentTemplateBlock.transform.position = dragPosition;
                    }

                    //버튼 땠을때 드래그 감지 해제
                    if (Input.GetMouseButtonUp(0))
                    {
                        currentTemplateBlock.transform.position = dragPosition;
                        isDrag = false;
                        print("BB");
                    }

                    if (isDrag)
                    {
                        //클릭다운된 좌표와 클릭업 된 좌표의 값을 빼서 임의의 값으로 나누어 currentTemplateBlock에 더해줌
                        //0.001f 값은 grid 메서드에서 근사치 오류 때문에 더해줌
                        dragPosition = PlaceCubeNear(new Vector3(
                            initPlayerPosition.x + (Input.mousePosition.x - dragPos.x) / 50.0f + 0.001f,
                            0.5f,
                            initPlayerPosition.z + (Input.mousePosition.y - dragPos.y) / 50.0f + 0.001f
                            ));
                    }

                    //블락 설치 버튼
                    if (Input.GetMouseButtonDown(2))
                    {
                        photonView.RPC("PlaceBlock", RpcTarget.All);
                    }
                }
            }
        }

        //블락위치를 확정하여 생성함
        [PunRPC]
        private void PlaceBlock() {
            if (canBuild && currentTemplateBlock != null)
            {
                //빌딩 모드에서 마우스 좌클릭시 블락 설치되는 부분 
                GameObject newBlock = Instantiate(blockPrefab, currentTemplateBlock.transform.position, Quaternion.identity);
            }
        }

        //Grid.cs 에서 클릭된 포지션의 grid 근삿값을 리턴함
        private Vector3 PlaceCubeNear(Vector3 clickPoint) {
            var finalPosition = grid.GetNearestPointOnGrid(clickPoint);
            
            return finalPosition;
        }
    }
}