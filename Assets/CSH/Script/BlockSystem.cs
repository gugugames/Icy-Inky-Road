/*
 * BlockSystem.cs
 * 
 * 요약
 * 모든 Block Type의 속성을 관리한다.
 * 각 Block은 ID, 이름, Material을 갖고 BlockSystem은 ID로 바로 찾을 수 있는 Dictionary 구조를 포함한다.
 * 
 * 수정
 * BlockSystem이 필요한 이유를 모르겠다. 각 Block을 Prefab으로 만들어버리면 따로 Block 종류 전체를 관리할 필요성이 없어지는데. 
 * 또한 모든 Block이 BlockSystem.cs 스크립트를 포함하는 이유도 불분명하다.
 * Cube Prefab 이름을 BlockWall 등으로 고쳐야 한다. 
 * 
 * Map, Grid, Block, Wall 디자인에 전체적으로 큰 수정이 필요하다.
 * 먼저 Block이라는 모든 Block간의 상호작용을 event로서 수신할 수 있는 abstract class 또는 interface를 구현한다.
 * 그 후 Block을 상속받아 Wall(Undestroyable), Destroyable, Movable Block을 만드는 게 좋아보인다. 
 * 이렇게 하면 기존의 BlockSystem 없이 Script 하나만 Component로 추가함으로서 하나의 Block과 그 기능을 모두 표현할 수 있다. 
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ClientLibrary
{

    public class BlockSystem : MonoBehaviour
    {

        [SerializeField]
        private BlockType[] allBlockTypes;

        [HideInInspector]
        public Dictionary<int, Block> allBlocks = new Dictionary<int, Block>();

        private void Awake() {
            for (int i = 0; i < allBlockTypes.Length; i++) {
                BlockType newBlockType = allBlockTypes[i];
                Block newBlock = new Block(i, newBlockType.blockName, newBlockType.blockMat);
                allBlocks[i] = newBlock;
                //Debug.Log("Block added to dictionary " + allBlocks[i].blockName);
            }
        }
    }

    public class Block
    {
        public int blockID;
        public string blockName;
        public Material blockMaterial;

        public Block(int id, string name, Material mat) {
            blockID = id;
            blockName = name;
            blockMaterial = mat;
        }
    }

    [Serializable]
    public struct BlockType
    {
        public string blockName;
        public Material blockMat;
    }
}