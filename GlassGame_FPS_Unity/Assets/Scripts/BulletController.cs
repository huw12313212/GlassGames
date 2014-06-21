using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {
	public float lifeTime = 3.0f;
	private float alreadyLiveTime;
	// Use this for initialization
	void Start () {
		//init
		alreadyLiveTime = 0.0f;

	}


	// Update is called once per frame
	void Update () {
		//delete

		alreadyLiveTime += Time.deltaTime;
		if (alreadyLiveTime > lifeTime)
		{
			GameObject.Destroy (gameObject);
		}

	}
}
