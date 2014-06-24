using UnityEngine;
using System.Collections;

public class CameraOffset : MonoBehaviour {

	public GameObject targetCamera;
	public GameObject lookAt;

	public int CountDown = 90;

	public void CheckOffset()
	{
		Vector3 difVec3 = lookAt.transform.position - targetCamera.transform.position;
		difVec3.Normalize ();
		float targetRotationY =  Mathf.Atan2(difVec3.x,difVec3.z);
		targetRotationY = (float)(targetRotationY/ Mathf.PI * 180);

		Vector3 vec3 = targetCamera.transform.forward;

		double data = Mathf.Atan2(vec3.x,vec3.z);


		double result = data / Mathf.PI * 180;

		Vector3 rotate = transform.localRotation.eulerAngles;
		rotate.y = - (float)result + (float)targetRotationY;

		transform.localRotation = Quaternion.Euler(rotate);

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//CheckOffset ();

		//
		if (CountDown > 0) 
		{
			CountDown--;
			if(CountDown<=0)
			{
				CheckOffset();
			}
		}

	}
}
