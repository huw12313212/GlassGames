using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public GameObject bullet;
	public float moveSpeed = 5.0f;
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
				//shoot
				shoot();

				Debug.Log ("Touch!");
				break; //here ends the 1st case
				
			case TouchPhase.Ended: //here begins the 2nd case
				break;
			}
		}
	}

	public void shoot(){
		//shoot
		Instantiate(bullet,transform.position,transform.rotation);

	}
}
