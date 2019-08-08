using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using System.Collections.Generic;		//Allows us to use Lists. 
using UnityEngine.UI;                   //Allows us to use UI.
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

    //게임 스테이트는 3가지로 분류
    public enum GameState
    {
        PREPARE,
        PLAY,
        END
    }
    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        //DontDestroyOnLoad(gameObject);
    }


    //스테이트가 prepare 일때
    //플레이어는 작동x
    //preparetimer 는 작동
    //prepare building 이 작동
    //
    public void StateManager()
    {

    }

    public void PreparationBuilding()
    {
        //설치가능지역을 보여주는 그리드
        
    }



    //Update is called every frame.
    void Update()
    {

    }
}