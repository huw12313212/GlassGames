using UnityEngine;
using System.Collections;

public class GyroCamera : MonoBehaviour {
	Vector3 initialDirection;
	bool setInitialDirection;
	Vector3 identityAngle;
	// Use this for initialization
	void Start () {

		//init 
		setInitialDirection = false;
		identityAngle = new Vector3 (0, 0, 0);

		//check enable
		bool gyoBool = SystemInfo.supportsGyroscope;
		//enable gtro
		if( gyoBool ) {
			Input.gyro.enabled = true;
		}
	
	}
	
	// Update is called once per frame
	void Update () {
		//update direction
		updateDirectionFromGyro ();
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
