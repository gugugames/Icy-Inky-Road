/**
 * UIPlayerRotator.cs
 * 드래그에 의한 Player Rotation을 담당하는 script.
 * OnMouseDrag 매서드를 구현한다.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerRotator : MonoBehaviour {

    public float rotationSpeed;

    private void OnMouseDrag()
    {
        float rotationX = Input.GetAxis("Mouse X") * rotationSpeed;
        transform.Rotate(0.0f, -rotationX, 0.0f);
    }
}
