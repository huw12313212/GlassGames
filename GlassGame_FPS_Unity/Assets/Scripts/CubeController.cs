using UnityEngine;
using System.Collections;

public class CubeController : MonoBehaviour {
	public Material cubeMaterial;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//transform.position += transform.forward * 1 * Time.deltaTime;

		//touch
		handleTouch ();
	}

	void handleTouch(){

		if (AndroidInput.touchCountSecondary == 1) {
			
			Touch touch = AndroidInput.GetSecondaryTouch(0);
			switch (touch.phase) { //following are 2 cases
				
			case TouchPhase.Began: //here begins the 1st case
				cubeMaterial.color = new Color((cubeMaterial.color.r+0.1f)%1.0f,(cubeMaterial.color.g+0.1f)%1.0f,(cubeMaterial.color.b+0.1f)%1.0f);


				Debug.Log ("Touch!");
				break; //here ends the 1st case
				
			case TouchPhase.Ended: //here begins the 2nd case
				break;
			}
		}
	}
}
