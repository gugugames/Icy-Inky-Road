using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ClientLibrary
{
    public class BuildingSystem : MonoBehaviour
    {
        private Grid grid;

        private Camera playerCamera;

        private bool buildModeOn = false;
        private bool canBuild = false;

        private BlockSystem bSys;

        [SerializeField]
        private LayerMask buildableSurfacesLayer;

        private Vector3 buildPos;

        private GameObject currentTemplateBlock;

        [SerializeField]
        private GameObject blockTemplatePrefab;
        [SerializeField]
        private GameObject blockPrefab;

        [SerializeField]
        private Material templateMaterial;

        private int blockSelectCounter = 0;

        bool isDrag = false;
        private Vector3 dragPos;
        Vector3 dragPosition;
        Vector3 initPlayerPosition;

        private void Start() {
            grid = FindObjectOfType<Grid>();
            bSys = GetComponent<BlockSystem>();
            playerCamera = FindObjectOfType<Camera>();
        }
        Vector3 temp;
        private void Update() {

            //E버튼 클릭시 빌딩 모드 실행, 다시 E 클릭시 빌딩모드 해제
            if (Input.GetKeyDown("e")) {
                buildModeOn = !buildModeOn;

                if (buildModeOn) {
                    canBuild = true;
                    //Cursor.lockState = CursorLockMode.Locked;
                }
                else {
                    canBuild = false;
                    Cursor.lockState = CursorLockMode.None;
                }
            }

            if (Input.GetKeyDown("r")) {
                blockSelectCounter++;
                if (blockSelectCounter >= bSys.allBlocks.Count) blockSelectCounter = 0;
            }

            if (buildModeOn) {
                
                RaycastHit buildPosHit;

                if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out buildPosHit, 10, buildableSurfacesLayer)) {
                    Vector3 point = buildPosHit.point;
                    temp = point;
                    buildPos = PlaceCubeNear(point);//new Vector3(Mathf.Round(point.x), Mathf.Round(point.y), Mathf.Round(point.z));
                    buildPos.y = 0;
                    
                }
                else {
                    if(currentTemplateBlock != null)
                        Destroy(currentTemplateBlock.gameObject);
                    
                }
            }

            if (!buildModeOn && currentTemplateBlock != null) {
                Destroy(currentTemplateBlock.gameObject);
                canBuild = false;
            }

            //Block 템플릿 실행
            if (Input.GetKeyDown("e") && currentTemplateBlock == null) {
                //이부분이 빌딩모드때 보이는 가상 블락 실행부분
                currentTemplateBlock = Instantiate(blockTemplatePrefab, PlaceCubeNear(buildPos), Quaternion.identity);
                currentTemplateBlock.GetComponent<MeshRenderer>().material = templateMaterial;
                print("CC");
            }
            //canBuild
            
            if (canBuild && currentTemplateBlock != null) {
                //currentTemplateBlock.transform.position = PlaceCubeNear(buildPos);
                //버튼 눌렀을때 드래그 감지
                if (Input.GetMouseButtonDown(0)) {
                    isDrag = true;
                    dragPos = Input.mousePosition;
                    initPlayerPosition.x = currentTemplateBlock.transform.position.x;
                    initPlayerPosition.z = currentTemplateBlock.transform.position.z;
                    
                    //currentTemplateBlock.transform.position = buildPos;
                    print("AA");
                }

                if (Input.GetMouseButton(0)) {
                    currentTemplateBlock.transform.position = dragPosition;
                }

                //버튼 땠을때 드래그 감지 해제
                if (Input.GetMouseButtonUp(0)) {
                    isDrag = false;
                    print("BB");
                }
                
                if (isDrag) {
                    //클릭다운된 좌표와 클릭업 된 좌표의 값을 빼서 임의의 값으로 나누어 currentTemplateBlock에 더해줌
                    dragPosition = PlaceCubeNear(new Vector3(
                        initPlayerPosition.x + (Input.mousePosition.x - dragPos.x) / 50.0f,
                        0.5f,
                        initPlayerPosition.z + (Input.mousePosition.y - dragPos.y) / 50.0f
                        ));
                    //한번 움직이고 나서 dragPos의 값을 초기화 시켜줌
                    //dragPos = Input.mousePosition;

                    print("initPlayerPosition : " + initPlayerPosition);
                    //print("dragPos.x : " + dragPos.x);
                    print("(Input.mousePosition.x - dragPos.x) : " + initPlayerPosition.z + (Input.mousePosition.y - dragPos.y) / 50.0f);
                }
                
                //블락 설치 버튼
                if (Input.GetMouseButtonDown(2)) {
                    PlaceBlock();
                }
            }
        }

        private void PlaceBlock() {
            //빌딩 모드에서 마우스 좌클릭시 블락 설치되는 부분 
            GameObject newBlock = Instantiate(blockPrefab, currentTemplateBlock.transform.position, Quaternion.identity);
            //Block tempBlock = bSys.allBlocks[blockSelectCounter];
            //newBlock.name = tempBlock.blockName + "-Block";
            //newBlock.GetComponent<MeshRenderer>().material = tempBlock.blockMaterial;
        }

        //Grid 연동부분
        private Vector3 PlaceCubeNear(Vector3 clickPoint) {
            var finalPosition = grid.GetNearestPointOnGrid(clickPoint);
            //GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = finalPosition;

            return finalPosition;
            //GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = nearPoint;
        }
    }
}