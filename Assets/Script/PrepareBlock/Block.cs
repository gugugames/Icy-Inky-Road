using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ClientLibrary
{
    public class Block : MonoBehaviour
    {


        public int blockID;
        public string blockName;
        public Material blockMaterial;

        public Block(int id, string name, Material mat)
        {
            blockID = id;
            blockName = name;
            blockMaterial = mat;
        }
    }

}
