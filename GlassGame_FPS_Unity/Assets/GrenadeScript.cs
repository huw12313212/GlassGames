using UnityEngine;
using System.Collections;

public class GrenadeScript : MonoBehaviour {

	public float speed;
	public float boomTime;
	public float backTime;

	public Camera camera;
	public GameObject particleSystem;
	public GameObject grenadeModel;

	public WeaponManager weaponManager;

	public bool ready = true;


	private Vector3 initPosition;
	private Quaternion initRotation;

	public GrenadeBoomChecker boomChecker;

	public int boomDamage = 100;

	// Use this for initialization
	void Start () {
		initPosition = transform.localPosition;
		initRotation = transform.localRotation;
	}


	public void back()
	{
		particleSystem.SetActive(false);

		if (weaponManager.weaponType == WeaponManager.WeaponType.handGrenade) 
		{
			grenadeModel.SetActive (true);
		}
		grenadeModel.rigidbody.isKinematic = true;
		grenadeModel.rigidbody.useGravity = false;
		grenadeModel.transform.parent = transform;
		grenadeModel.transform.localPosition = initPosition;
		grenadeModel.transform.localRotation = initRotation;
		//grenadeModel.rigidbody.velocity = Vector3.zero;
		ready = true;
	}

	public void boom()
	{

		particleSystem.SetActive(true);
		grenadeModel.SetActive(false);
		particleSystem.transform.position = grenadeModel.transform.position;
		Invoke ("back", backTime);

		//trigger boom damage
		boomChecker.hurtAllMonsterInRange (boomDamage);

	}

	public void throwGrenade()
	{
		if (ready) 
		{
			ready = false;
			grenadeModel.transform.parent = null;
			Vector3 v3 = camera.transform.forward * speed;
			grenadeModel.rigidbody.isKinematic = false;
			grenadeModel.rigidbody.useGravity = true;
			grenadeModel.rigidbody.velocity = v3;

			Invoke ("boom", boomTime);
		}

	}


}
