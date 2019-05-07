using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using System.Collections.Generic;		//Allows us to use Lists. 
using UnityEngine.UI;					//Allows us to use UI.


    public class GameManager : MonoBehaviour
    {
        public float turnDelay = 0.1f;                          //Delay between each Player turn.
        public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        [HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.

        //private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
        private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.


        //Awake is always called before any Start functions
        void Awake() {
            Time.timeScale = 1.0f;

            //RePlayButton.SetActive(false);
            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            //Get a component reference to the attached BoardManager script
            //boardScript = GetComponent<BoardManager>();

        }

        //Update is called every frame.
        void Update() {
            //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
            //if (playersTurn || doingSetup)

            //    //If any of these are true, return and do not start MoveEnemies.
            //    return;

            //StartCoroutine(TurnDelay());
            //if(boardScript.gridPositions == Player.instance.Position)
            //int gridPosition = (int)Player.instance.Position.x * boardScript.columns + (int)Player.instance.Position.y;
            //print("gridPosition : " + gridPosition);
            //boardScript.floorObject[gridPosition].GetComponent<SpriteRenderer>().color = new Color(0,0,0);
            //print("boardScript.floorObject[gridPosition].GetComponent<SpriteRenderer>().color : " + boardScript.floorObject[gridPosition].GetComponent<SpriteRenderer>().color);
        }

        //Coroutine to move enemies in sequence.
        IEnumerator TurnDelay() {

            //Wait for turnDelay seconds, defaults to .1 (100 ms).
            yield return new WaitForSeconds(turnDelay);

            playersTurn = true;
            print("playersTurn : " + playersTurn);
        }
    }