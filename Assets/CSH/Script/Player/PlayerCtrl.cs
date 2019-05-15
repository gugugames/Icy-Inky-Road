using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClientLibrary;

public class PlayerCtrl : MonoBehaviour
{
    public int horizontal = 0;     //Used to store the horizontal move direction.
    public int vertical = 0;
    public int queueLimit=2;


    private int inputXDir;
    private int inputYDir;


    private BoxCollider boxCollider;      //The BoxCollider2D component attached to this object.
    private Rigidbody rigidbody;               //The Rigidbody2D component attached to this object.
    private CharacterController controller;

    public LayerMask blockingLayer;         //Layer on which collision will be checked.
    public float speed = 6.0F;

    private bool playerTurn;
    private Vector3 moveDirection = Vector3.zero;
    private Queue<Vector3> queueMoveDirection = new Queue<Vector3>();
    private IEnumerator smoothMovement;
    private Vector3 currentGrid;
    private ClientLibrary.Grid grid;
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif

    public static PlayerCtrl instance = null;

    public Vector3 Position
    {
        get {
            return transform.position;
        }
    }

    void Awake() {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
    }
    private void Start() {
        //smoothMovment null 초기화
        smoothMovement = SmoothMovement(null);
        
        //Get a component reference to this object's BoxCollider2D
        boxCollider = GetComponent<BoxCollider>();

        //Get a component reference to this object's Rigidbody2D
        rigidbody = GetComponent<Rigidbody>();

        //grid 초기화
        grid = ClientLibrary.Grid.instance;

        //currentGrid 초기화
        currentGrid = grid.GetNearestPointOnGrid(transform.position);

        controller = GetComponent<CharacterController>();
    }


    private void Update() {
        

        MoveCtrl();
    }


    private void MoveCtrl() {
        //If it's not the player's turn, exit the function.
        //moveDirection.x = 0;     //Used to store the horizontal move direction.
        //moveDirection.z = 0;      //Used to store the vertical move direction.

        //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER

        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
        moveDirection.x = (int)(Input.GetAxisRaw("Horizontal"));

        //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
        moveDirection.z = (int)(Input.GetAxisRaw("Vertical"));

        //Check if moving horizontally, if so set vertical to zero.
        if (moveDirection.x != 0) {
            moveDirection.z = 0;
        }

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, moveDirection, out hit, Mathf.Infinity, blockingLayer))
        {
            Debug.DrawRay(transform.position, moveDirection * hit.distance, Color.red);
            Debug.Log("Did Hit");
        }

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
//Check if Input has registered more than zero touches
if (Input.touchCount > 0)
{
	//Store the first touch detected.
	Touch myTouch = Input.touches[0];
				
	//Check if the phase of that touch equals Began
	if (myTouch.phase == TouchPhase.Began)
	{
		//If so, set touchOrigin to the position of that touch
		touchOrigin = myTouch.position;
	}
				
	//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
	else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
	{
		//Set touchEnd to equal the position of this touch
		Vector2 touchEnd = myTouch.position;
					
		//Calculate the difference between the beginning and end of the touch on the x axis.
		float x = touchEnd.x - touchOrigin.x;
					
		//Calculate the difference between the beginning and end of the touch on the y axis.
		float y = touchEnd.y - touchOrigin.y;
					
		//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
		touchOrigin.x = -1;
					
		//Check if the difference along the x axis is greater than the difference along the y axis.
		if (Mathf.Abs(x) > Mathf.Abs(y))
			//If x is greater than zero, set horizontal to 1, otherwise set it to -1
			moveDirection.x = x > 0 ? 1 : -1;
		else
			//If y is greater than zero, set horizontal to 1, otherwise set it to -1
			moveDirection.z = y > 0 ? 1 : -1;
	}
}
			
#endif //End of mobile platform dependendent compilation section started above with #elif
        if ((moveDirection.x != 0 || moveDirection.z != 0) && Input.anyKeyDown) {
            //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
            //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
            AttemptMove(moveDirection);
        }

    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.transform.tag == "Wall") {
            
        }

    }

    private void StopMove()
    {
        //if(smoothMovement != null)
        try
        {
            StopCoroutine(smoothMovement);
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
            throw;
        }
            
        playerTurn = false;
        print("stopmove");
    }

    public Vector3 QueueMove{
        get {
            return queueMoveDirection.Dequeue();
        }
        set {
            queueMoveDirection.Enqueue(value);
            print("asdasd : " + queueMoveDirection);
        }
    }
        
        

    private void AttemptMove(Vector3 moveDirection) {
        
        if (queueMoveDirection.Count < queueLimit)
        {
            QueueMove = this.moveDirection;
            print("queueMoveDirection count : " + queueMoveDirection.Count);
            print("Enqueue : " + this.moveDirection);
        }
        if (playerTurn == false && queueMoveDirection.Count != 0)
        {
            playerTurn = true;
            smoothMovement = SmoothMovement(QueueMove);
            StartCoroutine(smoothMovement);
            print("start Coroutine");
        }

        
    }


    //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    protected virtual IEnumerator SmoothMovement(Vector3? dir = null) {
        //dir 이 null 이면 리턴
        if (dir == null) yield return null;
        
        
        while (true)
        {
            if (CheckForward(dir.Value))
            {
                    Debug.Log("Did Hit");
                    StopMove();
            }
            //dir 이 null 이 아니면 실행
            transform.Translate(dir.Value * 0.1f);
                //print("SmoothMovement");
                yield return new WaitForSeconds(0.1f);

        }
    }

    private bool CheckForward(Vector3 dir)
    {

        try
        {
            //다음 Grid 로 이동했을 시 true
            if (!grid.BoolCurrentPosition(GetNextGrid(dir)))
            {
                grid.BoolCurrentPosition(GetCurrnetGrid(), true);
                print("CheckForward true check");
                //다음 grid 좌표 bool true로 변경

                //currentGrid = GetNextGrid(dir);
                return true;
            }
            else
            {
                //print("CheckForward error : " + "currentGrid = " + currentGrid + "GetCurrnetGrid() = " + GetCurrnetGrid());
                return false;
            }
        }
        catch(Exception ex)
        {
            print("error");
            return false;
        }
        
    }

    private Vector3 GetCurrnetGrid()
    {
        //print("GetCurrnetGrid : " + grid.GetNearestPointOnGrid(transform.position));
        return ClientLibrary.Grid.instance.GetNearestPointOnGrid(transform.position);
    }

    private Vector3 GetNextGrid(Vector3 dir)
    {
        //print("GetCurrnetGrid : " + grid.GetNearestPointOnGrid(transform.position + dir));
        return ClientLibrary.Grid.instance.GetNearestPointOnGrid(transform.position + dir);
    }


}

