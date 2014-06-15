using UnityEngine;
using System.Collections;

public class GyroCamera : MonoBehaviour {
	Vector3 initialDirection;
	bool setInitialDirection;
	Vector3 identityAngle;


	private bool gyroBool ;
	private Quaternion rotFix ;
	private Gyroscope gyro;


	// Use this for initialization
	void Start () {

		/*
		//init 
		setInitialDirection = false;
		identityAngle = new Vector3 (0, 0, 0);

		//check enable
		bool gyoBool = SystemInfo.supportsGyroscope;
		//enable gtro
		if( gyoBool ) {
			Input.gyro.enabled = true;
		}*/

		Transform originalParent = transform.parent; // check if this transform has a parent
		GameObject camParent = new GameObject ("camParent"); // make a new parent
		camParent.transform.position = transform.position; // move the new parent to this transform position
		transform.parent = camParent.transform; // make this transform a child of the new parent
		camParent.transform.parent = originalParent; // make the new parent a child of the original parent
		
		gyroBool = Input.isGyroAvailable;
		
		if (gyroBool) {
			
			gyro = Input.gyro;
			gyro.enabled = true;
			Debug.Log("Orientation:"+Screen.orientation);


				camParent.transform.eulerAngles = new Vector3(90,180,0);

				rotFix =new  Quaternion(0f,0f,1f,0f);

			//Screen.sleepTimeout = 0;
		} else {
			print("NO GYRO");
		}
	
	}
	
	// Update is called once per frame
	void Update () {
		//update direction
		//updateDirectionFromGyro ();

		if (gyroBool) {
			Quaternion camRot = gyro.attitude * rotFix;
			transform.localRotation = camRot;
		}
	}

	void updateDirectionFromGyro(){
		Debug.Log ("Gyro:"+Input.gyro.attitude);

		//get initial Rotation
		if((setInitialDirection == false) && (Input.gyro.attitude.eulerAngles != identityAngle)){
			//initial
			initialDirection = Input.gyro.attitude.eulerAngles;
			setInitialDirection = true;
		}

		//calc rotate angle
		Vector3 rotateAngle = Input.gyro.attitude.eulerAngles - initialDirection;

		//set camera rotation
		//transform.rotation = ConvertRotation(Quaternion.Euler(rotateAngle))* GetRotFix();
		//transform.rotation = ConvertRotation(Input.gyro.attitude)* GetRotFix();
		transform.rotation = Quaternion.Euler(rotateAngle);

		//debug
		Debug.Log ("Initial:"+initialDirection);
		Debug.Log ("Rotation:"+transform.rotation.eulerAngles);

	}
	
	private static Quaternion ConvertRotation(Quaternion q)
	{
		return new Quaternion(q.x, q.y, -q.z, -q.w);
	}
	
	private Quaternion GetRotFix()
	{
		if (Screen.orientation == ScreenOrientation.Portrait)
			return Quaternion.identity;
		if (Screen.orientation == ScreenOrientation.LandscapeLeft
		    || Screen.orientation == ScreenOrientation.Landscape)
			return Quaternion.Euler(0, 0, -90);
		if (Screen.orientation == ScreenOrientation.LandscapeRight)
			return Quaternion.Euler(0, 0, 90);
		if (Screen.orientation == ScreenOrientation.PortraitUpsideDown)
			return Quaternion.Euler(0, 0, 180);
		return Quaternion.identity;
	}
}
