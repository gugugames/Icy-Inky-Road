/**
 * UIController.cs
 * 
 * MVC(Model View Controller) 모델의 Controller 부분을 담당한다. 
 * Interaction을 위한 Callback을 구현한다.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour { 
    public float fadeTime; // fade in/out에 걸리는 시간. 변하지 않는 값. 

    public void Enable(Transform transform)
    {
        transform.gameObject.SetActive(true);
    }

    public void Disable(Transform transform)
    {
        transform.gameObject.SetActive(false);
    }

    public void LoadScene(int sceneBuildIndex)
    {
        SceneManager.LoadScene(sceneBuildIndex);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    IEnumerator FadeInCoroutine(Transform transform)
    {
        Enable(transform);
        float fadeInTime = 0.0f;
        Graphic graphic = transform.GetComponent<Text>() as Graphic;
        if (!graphic)
        {
            graphic = transform.GetComponent<Image>() as Graphic;
            if (!graphic)
            {
                yield break;
            }
        }
        while (fadeInTime < fadeTime)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, fadeInTime / fadeTime);
            fadeInTime += Time.deltaTime;
            yield return null;
        }
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1.0f);
    }

    IEnumerator FadeOutCoroutine(Transform transform)
    {
        float fadeOutTime = fadeTime;
        Graphic graphic = transform.GetComponent<Text>() as Graphic;
        if (!graphic)
        {
            graphic = transform.GetComponent<Image>() as Graphic;
            if (!graphic)
            {
                yield break;
            }
        }
        while (fadeOutTime > 0)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, fadeOutTime / fadeTime);
            fadeOutTime -= Time.deltaTime;
            yield return null;
        }
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0.0f);
        Disable(transform);
    }
    
    /**
     * Image 또는 Text Component를 fade in 시킨다.
     */
    public void FadeIn(Transform transform)
    {
        StartCoroutine(FadeInCoroutine(transform));
    }

    /**
     * Image 또는 Text Component를 fade out 시킨다.
     */
    public void FadeOut(Transform transform)
    {
        StartCoroutine(FadeOutCoroutine(transform));
    }
}
