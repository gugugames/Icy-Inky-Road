using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ClientLibrary;
using ExitGames.Client.Photon;


public class PlayerCtrl : MonoBehaviourPun
{
    public enum PlayerTeam
    {
        empty,
        A,
        B
    }

    //플레이 시작시 설정해야하는 값
    //기본 A
    public PlayerTeam playerTeam = PlayerTeam.A;

    public int horizontal = 0;     //Used to store the horizontal move direction.
    public int vertical = 0;
    public int queueLimit=1;
    public TextMesh tm;

    private int inputXDir;
    private int inputYDir;
    private int playerScore = 0;

    private BoxCollider boxCollider;      //The BoxCollider2D component attached to this object.
    private CharacterController controller;

    public LayerMask blockingLayer;         //Layer on which collision will be checked.
    public float speed = 0.1f;

    private bool playerTurn;
    private Vector3 moveDirection = Vector3.zero;
    private Queue<Vector3> queueMoveDirection = new Queue<Vector3>();
    private IEnumerator smoothMovement;
    private Vector3 currentGrid;
    private ClientLibrary.Grid grid;
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif
    //public static PlayerCtrl instance = null;

    public Vector3 Position
    {
        get {
            return transform.position;
        }
    }

    void Awake() {
        //Check if instance already exists
        /*if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);*/
    }
    [PunRPC]
    public void ChangeColor()
    {
        playerTeam = PlayerTeam.B;
        transform.Find("Particle System").GetComponent<ParticlePainter>().brush.splatChannel = 1;
    }

    private void Start() {
        if(PhotonNetwork.PlayerList[0].UserId == PhotonNetwork.AuthValues.UserId && photonView.IsMine)
        {
            photonView.RPC("ChangeColor", RpcTarget.All);
        }
        if (!photonView.IsMine)
        {
            return;
        }
        else
        {
            //smoothMovment null 초기화
            smoothMovement = SmoothMovement(null);

            //Get a component reference to this object's BoxCollider2D
            boxCollider = GetComponent<BoxCollider>();

            //grid 초기화
            grid = ClientLibrary.Grid.instance;

            //currentGrid 초기화
            currentGrid = grid.GetNearestPointOnGrid(transform.position);

            controller = GetComponent<CharacterController>();

            //위치 초기화
            transform.position = grid.GetCurrentGrid(transform.position) + new Vector3(0.001f, 0, 0.001f);//반올림 error 보간값
        }
    }


    private void Update() {
        if (!photonView.IsMine)
        {
            return;
        }
        else
        {
            MoveCtrl();
        }
    }


    private void MoveCtrl() {
        playerScore += 1;

        ExitGames.Client.Photon.Hashtable PlayerCustomProps = new ExitGames.Client.Photon.Hashtable();
        PlayerCustomProps["Score"] = playerScore;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerCustomProps);

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
            //Debug.Log("Did Hit");
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

            playerTurn = false;
            AttemptMove();
            //print("stopmove");
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
            throw;
        }
            
        
    }

    public Vector3 QueueMove{
        get {
            return queueMoveDirection.Dequeue();
        }
        set {
            queueMoveDirection.Enqueue(value);
            //print("asdasd : " + queueMoveDirection);
        }
    }

    private void AttemptMove(Vector3? moveDirection = null) {

        if (queueMoveDirection.Count < queueLimit && moveDirection != null)
        {
            QueueMove = this.moveDirection;
            tm.text = PreDirection(this.moveDirection);
            //print("Enqueue : " + queueMoveDirection.Count);
            //print("queueMoveDirection count : " + queueMoveDirection.Count);
            //print("Enqueue : " + this.moveDirection);
        }
        if (playerTurn == false && queueMoveDirection.Count != 0)
        {
            //print("Dequeue : " + queueMoveDirection.Count);
            playerTurn = true;
            smoothMovement = SmoothMovement(QueueMove);
            StartCoroutine(smoothMovement);
            //print("start Coroutine");
        }
    }

    private string PreDirection(Vector3 dir)
    {
        if (dir.x > 0) return "→";
        if (dir.x < 0) return "←";
        if (dir.z > 0) return "↑";
        if (dir.z < 0) return "↓";
        return "";
    }


    //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    protected virtual IEnumerator SmoothMovement(Vector3? dir = null) {
        //dir 이 null 이면 리턴
        if (dir == null) yield return null;
        
        while (true)
        {
            //print("AA : " + Vector3.Distance(transform.position, grid.GetCurrnetGrid(transform.position)));
            if (CheckForward(dir.Value))
            {
                if (Vector3.Distance(transform.position, grid.GetCurrentGrid(transform.position)) > 0.1f)
                {
                    
                    transform.Translate(dir.Value * speed);
                }
                else
                {
                    CheckForward(dir.Value);
                    transform.position = grid.GetCurrentGrid(transform.position) + new Vector3(0.001f, 0, 0.001f);
                    StopMove();
                    photonView.RPC("Calculate", RpcTarget.All, dir.Value);
                    //Debug.Log("Did Hit");
                }
            }
            else
            {
                //print("AA");
                photonView.RPC("Calculate2", RpcTarget.All, dir.Value);
                //grid.GetSetBoolPlayerOcuupationPosition(transform.position, playerTeam, true);
                if(grid.CheckWallPosition(grid.GetPreviousGrid(transform.position, dir.Value)) == false)
                {
                    photonView.RPC("Calculate3", RpcTarget.All, dir.Value);
                    //print("AA : " + grid.CheckWallPosition(grid.GetPreviousGrid(transform.position, dir.Value)));
                    //grid.GetSetBoolPlayerOcuupationPosition(grid.GetPreviousGrid(transform.position, dir.Value), playerTeam, false);

                }
                grid.SetMapShareArray(transform.position, playerTeam);
                //dir 이 null 이 아니면 실행
                transform.Translate(dir.Value * speed);
                //print("SmoothMovement");
            }
            //나중에 최적화를 위해 위치 변경
            //grid.GridShare(transform.position, "A");
            yield return null;

        }
    }

    [PunRPC]
    public void Calculate(Vector3? dir = null)
    {
        grid.GetSetBoolPlayerOcuupationPosition(transform.position, playerTeam, true);
        grid.GetSetBoolPlayerOcuupationPosition(grid.GetPreviousGrid(transform.position, dir.Value), playerTeam, false);
    }
    [PunRPC]
    public void Calculate2(Vector3? dir = null)
    {
        grid.GetSetBoolPlayerOcuupationPosition(transform.position, playerTeam, true);
    }
    [PunRPC]
    public void Calculate3(Vector3? dir = null)
    {
        grid.GetSetBoolPlayerOcuupationPosition(grid.GetPreviousGrid(transform.position, dir.Value), playerTeam, false);
    }

    private bool CheckForward(Vector3 dir)
    {
        //tm.text = Vector3.Distance(transform.position, grid.GetCurrnetGrid(transform.position, dir)) + "\n"
        //    + transform.position + "\n"
        //    + grid.GetCurrnetGrid(transform.position, dir) + "\n"
        //    + grid.GetCurrnetGrid(transform.position, dir) + "\n"
        //    + grid.GridPositionToArray(grid.GetNextGrid(transform.position, dir)) + "\n"
        //    + grid.BoolCurrentPosition(grid.GetNextGrid(transform.position, dir)) + "\n";
        try
        {
            //다음 Grid 로 이동했을 시 true
            if (grid.GetSetBoolWallPosition(grid.GetNextGrid(transform.position, dir)) || 
                grid.GetSetBoolPlayerOcuupationPosition(grid.GetNextGrid(transform.position, dir)))
            {

                //grid.BoolCurrentPosition(grid.GetCurrnetGrid(transform.position), true);

                //print("CheckForward true check");

                //다음 grid 좌표 bool true로 변경
                //currentGrid = GetNextGrid(dir);
                return true;
            }
            else
            {
                //grid.BoolCurrentPosition(grid.GetCurrnetGrid(transform.position), false);
                //print("CheckForward error : " + "currentGrid = " + currentGrid);
                return false;
            }
        }
        catch(Exception ex)
        {
            print("error : " + ex);
            return true;
        }
        
    }




}

