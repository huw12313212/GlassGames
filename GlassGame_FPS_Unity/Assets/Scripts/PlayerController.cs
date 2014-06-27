using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float moveSpeed;
	public float rotateSpeed;

	public Vector2 joyStickInput = new Vector2(0,0);
	public Vector2 joyStickInputRight = new Vector2 (0, 0);

	public GameObject offset;
	public Camera camera;

	public WeaponManager weaponManager;
	public float touchPadMoveSpeed;

	public bool triggerTap = false;


	public float forwordRatio;
	public float backRatio;

	public float tiltData;

	private float forwordPosition;
	
	private float backPosition;


	// Use this for initialization
	void Start () {

		backPosition = AndroidInput.secondaryTouchWidth *backRatio;
		forwordPosition = AndroidInput.secondaryTouchWidth *forwordRatio;
	}
	
	// Update is called once per frame
	void Update () {


		handleMove();
		handleJoystick();
		handleJoystick2();

		handleTouch ();


	}

	bool touched = false;
	bool longpressed = false;

	void handleMove()
	{
		if(touched)
		{
			if (touchCounter > 0) 
			{
				touchCounter-= Time.deltaTime;
			} 
			else 
			{
				if(triggerTap)
				{
					longpressed = true;
					triggerTap = false;
				}
			}

			if(longpressed)
			{
				if(lastTouch.position.x < backPosition)
				{
					joyStickInput.y = -1*touchPadMoveSpeed;
				}
				else if(lastTouch.position.x > backPosition)
				{
					joyStickInput.y = 1*touchPadMoveSpeed;
				}
				else
				{
					joyStickInput.y = 0;
				}
			}


		}
	}

	public float moveDelay;
	private float touchCounter = 0;

	private Touch lastTouch;

	void handleJoystick2()
	{
		float rotateValue = joyStickInputRight.x * rotateSpeed * Time.deltaTime;
		offset.transform.Rotate (new Vector3 (0, rotateValue, 0));
	}

	void handleJoystick()
	{
		Vector3 CurrentVec3 = camera.transform.forward;
		CurrentVec3.y = 0;
		CurrentVec3.Normalize();
		
		Vector3 RotateVec3 = camera.transform.right;
		RotateVec3.y = 0;
		RotateVec3.Normalize ();
		
		Vector3 v = (joyStickInput.y * CurrentVec3 + (joyStickInput.x + tiltData) * RotateVec3);
		
		gameObject.rigidbody.velocity = v * moveSpeed ;
	}



	void handleTouch(){

		if (AndroidInput.touchCountSecondary == 1) {
			
			Touch touch = AndroidInput.GetSecondaryTouch(0);

		

			switch (touch.phase) { //following are 2 cases
				
			case TouchPhase.Began: //here begins the 1st case
				//shoot
				//shoot();	
				touchCounter = moveDelay;
				lastTouch = touch;
				triggerTap = true;
				touched=true;


				break; //here ends the 1st case
				
			case TouchPhase.Ended: //here begins the 2nd case

				if(triggerTap)shoot();

				triggerTap = false;
				touched = false;
				joyStickInput.y = 0;
				longpressed = false;

				break;

			case TouchPhase.Moved:

				Vector2 delta = touch.deltaPosition;

				if(delta.magnitude > 15)
				{
					triggerTap = false;
				}

				if(delta.y < -15 && delta.x < 5 && delta.x>-5)
				{
					Application.Quit();

				}
				else if(delta.y > 15 && delta.x < 5 && delta.x>-5)
				{
//					weaponManager.SwitchWeapon();
					
				}
				else
				{
					/*
					Vector3 v = delta.x * camera.transform.forward * touchPadMoveSpeed;
					v.y = 0;
					rigidbody.velocity = v;*/

					if(delta.x > 15 || delta.x <-15)
					{
						transform.transform.Rotate(new Vector3(0,delta.x*touchPadRotateSpeed,0));
					}
				}



				break;
			}
		}
	}

	public float touchPadRotateSpeed;
	private Vector3 axis = new Vector3(0,1,0);


	public void shoot(){

		weaponManager.TriggerAttack ();
		//shoot
		//Instantiate(bullet,transform.position,transform.rotation);

	}
}
