/**
 * UIButtonStyle.cs
 * 
 * Button UI 요소에 지정한 Style을 적용한다.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class UIButtonStyle : UIStyle {

    public UIButtonStyleData styleData;
    
    private Button button;
    private Image image;

    private RectTransform rectTransform;

    private Transform icon;
    private RectTransform iconRectTransform;

    private Transform textbox;
    private Text text;
    private RectTransform textRectTransform;

    protected override void OnStyleUI()
    {
        base.OnStyleUI();

        button = GetComponent<Button>();
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        Navigation temp = new Navigation { mode = Navigation.Mode.None };
        button.navigation = temp; // navigation mode를 None으로 해야만 의도한 결과를 얻는다.

        // Image sprite, type 설정
        image.type = Image.Type.Sliced;
        
        // RectTransform width, height 설정
        rectTransform.sizeDelta = styleData.sizeDelta;
        
        // Icon 설정
        icon = transform.Find("Icon");
        if (icon != null)
        {
            iconRectTransform = icon.GetComponent<RectTransform>();

            // Icon anchor, position, width, height 설정
            iconRectTransform.anchorMax = styleData.iconAnchorMax;
            iconRectTransform.anchorMin = styleData.iconAnchorMin;
            iconRectTransform.anchoredPosition = styleData.iconAnchoredPosition;
            iconRectTransform.sizeDelta = styleData.iconSizeDelta; 
        }

        // Text 설정
        textbox = transform.Find("Text");
        if (textbox != null)
        {
            text = textbox.GetComponent<Text>();
            textRectTransform = textbox.GetComponent<RectTransform>();

            text.font = styleData.textFont;
            text.fontStyle = styleData.textFontStyle;
            text.fontSize = styleData.textFontSize;
            text.color = styleData.textColor;

            textRectTransform.anchorMax = styleData.textAnchorMax;
            textRectTransform.anchorMin = styleData.textAnchorMin;
        }
    }
}
