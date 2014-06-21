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


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		handleJoystick();
		handleJoystick2();

		handleTouch ();


	}

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
		
		Vector3 v = (joyStickInput.y * CurrentVec3 + joyStickInput.x * RotateVec3);
		
		gameObject.rigidbody.velocity = v * moveSpeed ;
	}



	void handleTouch(){

		if (AndroidInput.touchCountSecondary == 1) {
			
			Touch touch = AndroidInput.GetSecondaryTouch(0);



			switch (touch.phase) { //following are 2 cases
				
			case TouchPhase.Began: //here begins the 1st case
				//shoot
				//shoot();

				triggerTap = true;


				break; //here ends the 1st case
				
			case TouchPhase.Ended: //here begins the 2nd case

				if(triggerTap)shoot();

				triggerTap = false;

				break;

			case TouchPhase.Moved:

				Vector2 delta = touch.deltaPosition;

				if(delta.magnitude > 10)triggerTap = false;

				if(delta.y < -10 && delta.x < 5 && delta.x>-5)
				{
					Application.Quit();

				}
				else 
				{
					Vector3 v = delta.x * camera.transform.forward * touchPadMoveSpeed;
					v.y = 0;
					rigidbody.velocity = v;
				}

				break;
			}
		}
	}


	public void shoot(){

		weaponManager.TriggerAttack ();
		//shoot
		//Instantiate(bullet,transform.position,transform.rotation);

	}
}
