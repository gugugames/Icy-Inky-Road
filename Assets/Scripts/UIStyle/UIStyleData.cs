/**
 * UIStyleData.cs
 * 
 * UI 요소에 적용되는 Style을 정의한다.
 * Project window에서 "Create/UI/UI Style Data"를 통해
 * UIStyleData Scriptable Object를 Editor 상에서 만들 수 있다. 
 *
 * 같은 style을 공유하는 UI 요소들을 한번에 제어하고 싶을 때 편리하게 사용 가능하다.
 * 
 * Prototype 제작 기간동안은 sprite swap을 이용한 transition을 구현한다.
 * animation을 이용한 transition은 실제 개발 단계에서 변경한다. 
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "UI/UI Style Data")]
public class UIStyleData : ScriptableObject
{
    public Sprite normalSprite; // normal sprite
    public SpriteState buttonSpriteState; // highlighted, pressed, disabled sprite

    public Vector2 sizeDelta; // RectTransform width, height

    // Icon data
    public Vector2 iconAnchorMin;
    public Vector2 iconAnchorMax;
    public Vector2 iconAnchoredPosition;
    public Vector2 iconSizeDelta;


    // Text data
    public Font textFont;
    public FontStyle textFontStyle;
    public int textFontSize;
    public Color textColor;
    public Vector2 textAnchorMin;
    public Vector2 textAnchorMax;
}