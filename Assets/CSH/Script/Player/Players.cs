using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;


//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Players : MovingObject
{
    public int horizontal = 0;     //Used to store the horizontal move direction.
    public int vertical = 0;


#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif

    public static Players instance = null;

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




    //Start overrides the Start function of MovingObject
    protected override void Start() {

        //Call the Start function of the MovingObject base class.
        base.Start();
    }

    // update문 기존 이동 함수
    private void Update() {
        MoveCtrl();
    }

    private void MoveCtrl() {
        //If it's not the player's turn, exit the function.

        horizontal = 0;     //Used to store the horizontal move direction.
        vertical = 0;      //Used to store the vertical move direction.

        //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER

        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));

        //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        //Check if moving horizontally, if so set vertical to zero.
        if (horizontal != 0) {
            vertical = 0;
        }


        //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
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
				horizontal = x > 0 ? 1 : -1;
			else
				//If y is greater than zero, set horizontal to 1, otherwise set it to -1
				vertical = y > 0 ? 1 : -1;
		}
	}
			
#endif //End of mobile platform dependendent compilation section started above with #elif
        //Check if we have a non-zero value for horizontal or vertical
        if (horizontal != 0 || vertical != 0) {
            //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
            //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    //AttemptMove overrides the AttemptMove function in the base class MovingObject
    //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
    protected override void AttemptMove<T>(int xDir, int yDir) {
        //Set the playersTurn boolean of GameManager to false now that players turn is over.

        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        base.AttemptMove<T>(xDir, yDir);

            
    }



    //OnCantMove overrides the abstract function OnCantMove in MovingObject.
    //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
    protected override void OnCantMove<T>(T component) {
            
    }
}

