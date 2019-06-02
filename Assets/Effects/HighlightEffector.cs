/**
 * HighlightEffector.cs
 * 
 * Mouse Down, Up Event에 따라 GameObject를 Highlight시키는 script.
 * 다양한 구현이 가능하다. 
 * 1. Outline Shader 이용
 * 2. 단순히 _Color property를 변경
 * 3. Material 교체
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightEffector : MonoBehaviour {
    private Material mat;

    private Color originalColor;
    public Color highlightColor;

    private void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
        originalColor = mat.GetColor("_Color");
    }

    private void OnMouseDown()
    {
        mat.SetColor("_Color", highlightColor); 
    }

    private void OnMouseUp()
    {
        mat.SetColor("_Color", originalColor);
    }
    /*
    // 3. Material 교체
    private MeshRenderer meshRenderer;

    public Material highlightMaterial;
    private Material originalMaterial;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
    }

    private void OnMouseDown()
    {
        meshRenderer.material = highlightMaterial;
    }

    private void OnMouseUp()
    {
        meshRenderer.material = originalMaterial;
    }
    */
}