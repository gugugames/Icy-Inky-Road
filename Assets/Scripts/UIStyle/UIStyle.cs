/**
 * UIStyle.cs
 * 
 * UI 요소의 Style을 Editor 환경에서 랜더링하기 위한 Base Class이다. 
 * UIStyleData Scriptable Object에서 읽어들인 데이터를 이용해
 * 같은 Style이 적용된 UI 요소들이 Style을 공유할 수 있도록 한다.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIStyle : MonoBehaviour {
    public UIStyleData styleData;
    
    protected virtual void OnStyleUI()
    {
         
    }

    protected virtual void Update()
    {
        if (Application.isEditor)
        {
            OnStyleUI();
        }
    }
}
