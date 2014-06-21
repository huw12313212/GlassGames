using UnityEngine;
using System.Collections;

public class BulletShooter : MonoBehaviour {

	public GameObject spark;

	public bool firing = false;
	public float shootDuration = 0.5f;
	public float flashDuration = 0.02f;
	public bool singleFire = true;

	public GameObject pushBackObject;
	public Vector3 originRotate;
	public Vector3 pushRotate;
	
	public float pushBackTime = 0.02f;
	public float backTime = 0.5f;

	public GameObject bullet;
	public float bulletSpeed;

	private bool flashing = false;
	private float counter;

	public crossHairAnimate crossHairControll;

	// Use this for initialization
	void Start () {


		counter = shootDuration;
	}

	public void TriggerShoot()
	{
		spark.transform.Rotate (new Vector3(0,0,90));
		spark.SetActive (true);
		flashing = true;
		singleFire = false;

		counter = 0;

		crossHairControll.shoot = true;
		InitBulletAndShoot();
	}
	
	// Update is called once per frame
	void Update () {

		if (counter < shootDuration) 
		{
			counter+=Time.deltaTime;
		}

		CheckShoot();
		UpdatePowerBack();
		UpdateFlash ();
	}

	void InitBulletAndShoot()
	{
		GameObject nowBullet = Instantiate(bullet,transform.position,transform.rotation) as GameObject;
		nowBullet.rigidbody.velocity = transform.forward * bulletSpeed;
	}

	void UpdatePowerBack()
	{
		if (counter >= shootDuration) 
		{
			pushBackObject.transform.localRotation = Quaternion.Euler(originRotate);
		}
		else if(counter < pushBackTime)
		{
			Vector3 currentEuler = Vector3.Lerp(originRotate, pushRotate, counter/pushBackTime);
			pushBackObject.transform.localRotation = Quaternion.Euler(currentEuler);
		}
		else if(counter >= pushBackTime)
		{
			float ratio = (counter-pushBackTime) /(backTime-pushBackTime);
			Vector3 currentEuler = Vector3.Lerp(pushRotate, originRotate, ratio);
			pushBackObject.transform.localRotation = Quaternion.Euler(currentEuler);
		}
	}

	void CheckShoot()
	{
		if (firing||singleFire) 
		{
			if(counter>=shootDuration)
			{
				TriggerShoot();
			}
		} 
	}

	void UpdateFlash()
	{
		if (counter > flashDuration && flashing) 
		{
			flashing = false;
			spark.SetActive (false);
			
		}
	}
}
