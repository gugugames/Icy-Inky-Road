using UnityEngine;
using System;
using UnityEditor;

namespace ClientLibrary
{
    public class Grid : MonoBehaviour
    {
        [SerializeField]
        private float size = 1f;

        

        public Vector3 GetNearestPointOnGrid(Vector3 position) {
            position -= transform.position;

            float xCount = Mathf.RoundToInt(position.x / size);
            float yCount = Mathf.RoundToInt(position.y / size);
            float zCount = Mathf.RoundToInt(position.z / size);

            Vector3 result = new Vector3(
                (float)xCount * size + 0.5f ,
                (float)yCount * size + 0.5f,
                (float)zCount * size - 0.5f );

            result += transform.position;

            return result;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            for (float x = -5; x < 5; x += size) {
                for (float z = 5; z > -5; z -= size) {
                    var point = GetNearestPointOnGrid(new Vector3(x, 0f, z));
                    Gizmos.DrawSphere(point, 0.1f);
                }

            }
        }
    }

}

