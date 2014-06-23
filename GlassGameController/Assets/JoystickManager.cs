using UnityEngine;
using System.Collections;

public class JoystickManager : MonoBehaviour {

	//public GameObject joyStickRight;

	public bool playable = false;

	public CNJoystick joystickLeft;
	public CNJoystick joystickRight;

	public NetworkManager networkManager;

	public bool test;

	// Use this for initialization
	void Start () {
		joystickLeft.JoystickMovedEvent += moved;
		joystickLeft.FingerLiftedEvent += stopped;

		joystickRight.JoystickMovedEvent += rotated;
		joystickRight.FingerLiftedEvent += stopRotated;

	}

	void moved(Vector3 vec3)
	{
		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","move");
		commandJsonObject.AddField("x",vec3.x);
		commandJsonObject.AddField("y",-(vec3.y));

		if(Bluetooth.Instance().IsConnected()){
			Bluetooth.Instance().Send(commandJsonObject.ToString());
		}

		if (networkManager.isConnected)
		{
			networkManager.Send(commandJsonObject.ToString());
		}
		
	}

	void rotated(Vector3 vec3){
		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","rotate");
		commandJsonObject.AddField("x",vec3.x);
		commandJsonObject.AddField("y",-(vec3.y));
		
		if(Bluetooth.Instance().IsConnected()){
			Bluetooth.Instance().Send(commandJsonObject.ToString());
		}

		
		if (networkManager.isConnected)
		{
			networkManager.Send(commandJsonObject.ToString());
		}
		
	}

	void stopRotated(){
		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","rotate");
		commandJsonObject.AddField("x",0);
		commandJsonObject.AddField("y",0);
		
		if(Bluetooth.Instance().IsConnected()){
			Bluetooth.Instance().Send(commandJsonObject.ToString());
		}

		
		if (networkManager.isConnected)
		{
			networkManager.Send(commandJsonObject.ToString());
		}
	}

	void stopped(){
		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","move");
		commandJsonObject.AddField("x",0);
		commandJsonObject.AddField("y",0);
		
		if(Bluetooth.Instance().IsConnected()){
			Bluetooth.Instance().Send(commandJsonObject.ToString());
		}

		
		if (networkManager.isConnected)
		{
			networkManager.Send(commandJsonObject.ToString());
		}
	}



	// Update is called once per frame
	void Update () {
		if (!playable) 
		{
			if(Bluetooth.Instance().IsConnected()||networkManager.isConnected)
			{
				playable = true;
				joystickLeft.gameObject.SetActive(true);
				joystickRight.gameObject.SetActive(true);
			}
		}

		if (test) 
		{
			test = false;	

			JSONObject commandJsonObject = new JSONObject();
			commandJsonObject.AddField("command","move");
			commandJsonObject.AddField("x",0);
			commandJsonObject.AddField("y",-1);
			
			if (networkManager.isConnected)
			{
				networkManager.Send(commandJsonObject.ToString());
			}
		}
	}
}
