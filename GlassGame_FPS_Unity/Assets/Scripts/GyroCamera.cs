using UnityEngine;
using System.Collections;

public class GyroCamera : MonoBehaviour {

	private bool gyroBool ;
	private Quaternion rotFix ;
	private Gyroscope gyro;


	private bool isInitY =false;
	private float initY = 0;


	// Use this for initialization
	void Start () {

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

		if (gyroBool) {


			if (!isInitY) 
			{
				initY = gyro.attitude.y;
				isInitY = true;
				Debug.Log("initY:"+initY);
			}

			Vector3 faceVector = gyro.attitude.eulerAngles;
			faceVector.y -= initY;

			Quaternion camRot = Quaternion.Euler(faceVector) * rotFix;
			transform.localRotation = camRot;
		}
	}
	
}
