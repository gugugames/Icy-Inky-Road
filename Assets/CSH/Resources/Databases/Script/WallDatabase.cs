using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{
    public class WallDatabase : ScriptableObject
    {
        #region singleton
        private static WallDatabase m_Instance;
        public static WallDatabase Instance
        {
            get {
                if (m_Instance == null)
                    m_Instance = Resources.Load("Databases/WallDatabase") as WallDatabase;

                return m_Instance;
            }
        }
        #endregion

        public WallInfo[] walls;


        public WallInfo Get(int index) {
            return (walls[index]);
        }

        public WallInfo GetByID(int ID) {
            for (int i = 0; i < this.walls.Length; i++) {
                if (this.walls[i].ID == ID)
                    return this.walls[i];
            }
            return null;
        }
    }
}
    
