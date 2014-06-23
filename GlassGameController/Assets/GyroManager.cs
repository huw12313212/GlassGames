using UnityEngine;
using System.Collections;

public class GyroManager : MonoBehaviour {

	public float updateRate = 0.1f;

	public float counter =0;

	public CommunicationManager communicationManager;

	// Use this for initialization
	void Start () {
		Input.gyro.enabled = true;


	}

	public Quaternion lastQ;
	
	// Update is called once per frame
	void Update () {

		counter += Time.deltaTime;

		Quaternion rotation = Input.gyro.attitude;
		Vector3 acc = Input.acceleration;

		if(counter>updateRate)
		{
			counter = 0;
			lastQ = rotation;

			JSONObject json = new JSONObject();
			json.AddField("command","gyro");
			json.AddField("x",lastQ.x);
			json.AddField("y",lastQ.y);
			json.AddField("z",lastQ.z);
			json.AddField("w",lastQ.w);
			json.AddField("accX",acc.x);
			json.AddField("accY",acc.y);
			json.AddField("accZ",acc.z);
			communicationManager.SendJson(json);


		}


	}
}
