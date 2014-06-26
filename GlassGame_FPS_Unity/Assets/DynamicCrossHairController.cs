using UnityEngine;
using System.Collections;

public class DynamicCrossHairController : MonoBehaviour {


	public GameObject smallGun;
	public Camera camera;

	int plnaeMask;
	// Use this for initialization
	void Start () {

		plnaeMask = LayerMask.NameToLayer("Enemy");

	}
	
	// Update is called once per frame
	void Update () {

	
		RaycastHit hit;

		//Plane.
		/*
		Ray ray = new Ray(smallGun.transform.position,smallGun.transform.forward);

		float distance = 0;

		if(plane.Raycast(ray,out distance))
		{
			Vector3 position = ray.GetPoint(distance);
			Vector3 CrossHairScreenPosition = camera.WorldToScreenPoint(position);
			Debug.Log("camera:"+CrossHairScreenPosition);
		}*/
		//Physics.Raycast(

		if(Physics.Raycast(smallGun.transform.position, smallGun.transform.forward, out hit)){
			//if collidied


			Vector3 CrossHairScreenPosition = camera.WorldToScreenPoint(hit.point);
			//
			Debug.Log(hit.collider.gameObject.name+" aimed:"+CrossHairScreenPosition);

			CrossHairScreenPosition.x -= Screen.width/2;
			CrossHairScreenPosition.y -= Screen.height/2;

			CrossHairScreenPosition.x *= transform.localScale.x;
			CrossHairScreenPosition.y *= transform.localScale.y;
			CrossHairScreenPosition.z = transform.localPosition.z;

			transform.localPosition = CrossHairScreenPosition;
		}
		else
		Debug.Log("nothing");

	}
}
