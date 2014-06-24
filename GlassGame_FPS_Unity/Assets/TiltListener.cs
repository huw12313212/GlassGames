using UnityEngine;
using System.Collections;

public class TiltListener : MonoBehaviour {

	public Camera mainCamera;

	public float TitltData;

	public float Threashhold;
	public float maxHold;

	public float initSpeed;
	public float maxSpeed;
	
	public PlayerController playerController;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	 	Vector3 up = mainCamera.transform.up;
		Vector3 right = mainCamera.transform.right;
		Vector3 planeRight = right;

		planeRight.y = 0;
		planeRight.Normalize ();


		TitltData = Vector3.Angle(up, planeRight)-90;

		if (Mathf.Abs (TitltData) > Threashhold) 
		{
			if(TitltData>0)
			{
				float ratio = (TitltData-Threashhold) / (maxHold - Threashhold);
				float speed = ratio*maxSpeed + (1-ratio)*initSpeed;

				playerController.tiltData = -speed;
			}
			else if(TitltData<0)
			{
				float ratio = -(TitltData+Threashhold) / (maxHold - Threashhold);
				float speed = ratio*maxSpeed + (1-ratio)*initSpeed;


				playerController.tiltData = speed;
			}
		}
		else
		{
			playerController.tiltData = 0;
		}

	}

}
