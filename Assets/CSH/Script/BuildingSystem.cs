using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ClientLibrary
{
    public class BuildingSystem : MonoBehaviour
    {
        private Grid grid;

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

        private void Start() {
            grid = FindObjectOfType<Grid>();
            bSys = GetComponent<BlockSystem>();
            
        }

        private void Update() {

            //E버튼 클릭시 빌딩 모드 실행, 다시 E 클릭시 빌딩모드 해제
            if (Input.GetKeyDown("e")) {
                buildModeOn = !buildModeOn;

                if (buildModeOn) {
                    //Cursor.lockState = CursorLockMode.Locked;
                }
                else {
                    Cursor.lockState = CursorLockMode.None;
                }
            }

            if (Input.GetKeyDown("r")) {
                blockSelectCounter++;
                if (blockSelectCounter >= bSys.allBlocks.Count) blockSelectCounter = 0;
            }

            if (buildModeOn) {

                if (buildModeOn) {
                    buildPos = transform.position + transform.forward * 1.0f; //Mathf.Round(point.x), Mathf.Round(point.y), Mathf.Round(point.z));
                    canBuild = true;
                }
                else {
                    Destroy(currentTemplateBlock.gameObject);
                    canBuild = false;
                }
            }

            if (!buildModeOn && currentTemplateBlock != null) {
                Destroy(currentTemplateBlock.gameObject);
                canBuild = false;
            }

            //Block 템플릿 실행
            if (canBuild && currentTemplateBlock == null) {
                //이부분이 빌딩모드때 보이는 가상 블락 실행부분
                currentTemplateBlock = Instantiate(blockTemplatePrefab, PlaceCubeNear(buildPos), Quaternion.identity);
                currentTemplateBlock.GetComponent<MeshRenderer>().material = templateMaterial;
            }

            if (canBuild && currentTemplateBlock != null) {
                currentTemplateBlock.transform.position = PlaceCubeNear(buildPos);

                //블락 설치 버튼
                if (Input.GetMouseButtonDown(0)) {
                    PlaceBlock();
                }
            }
        }

        private void PlaceBlock() {
            //빌딩 모드에서 마우스 좌클릭시 블락 설치되는 부분 
            GameObject newBlock = Instantiate(blockPrefab, PlaceCubeNear(buildPos), Quaternion.identity);
            Block tempBlock = bSys.allBlocks[blockSelectCounter];
            newBlock.name = tempBlock.blockName + "-Block";
            newBlock.GetComponent<MeshRenderer>().material = tempBlock.blockMaterial;
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