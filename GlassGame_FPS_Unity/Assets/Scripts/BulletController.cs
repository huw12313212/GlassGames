using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {
	public float lifeTime = 3.0f;
	private float alreadyLiveTime;
	public float moveSpeed = 15.0f;
	public float yOffset = 1.8f;
	// Use this for initialization
	void Start () {
		//init
		alreadyLiveTime = 0.0f;
		initBullet ();
	}

	void initBullet(){
		//set position
		this.transform.position = new Vector3((float)this.transform.position.x,(float)(this.transform.position.y+yOffset),(float)this.transform.position.z);
		this.transform.position = this.transform.position + (this.transform.forward * moveSpeed * Time.deltaTime)*5;
	}

	// Update is called once per frame
	void Update () {
		//delete
		if (alreadyLiveTime > lifeTime) {
			//destroy
			GameObject.Destroy(this.gameObject);
		} else {
			//saved time
			alreadyLiveTime += Time.deltaTime;

			//move
			this.transform.position += this.transform.forward * moveSpeed * Time.deltaTime;
		}
	}
}
