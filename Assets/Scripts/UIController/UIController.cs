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

public class UIController : MonoBehaviour { 
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
}
