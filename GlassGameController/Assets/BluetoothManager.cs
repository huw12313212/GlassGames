using UnityEngine;
using System.Collections;

public class BluetoothManager : MonoBehaviour {

	//public GameObject joyStickRight;

	public CNJoystick joystickLeft;
	public CNJoystick joystickRight;
	public BluetoothGUI gui; 
	// Use this for initialization
	void Start () {
		joystickLeft.JoystickMovedEvent += moved;
		joystickLeft.FingerLiftedEvent += stopped;

		joystickRight.JoystickMovedEvent += rotated;
		joystickRight.FingerLiftedEvent += stopRotated;

	}

	void moved(Vector3 vec3)
	{
		//BluetoothGUI.Result = vec3+"";
		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","move");
		commandJsonObject.AddField("x",vec3.x);
		commandJsonObject.AddField("y",-(vec3.y));

		if(Bluetooth.Instance().IsConnected()){
			Bluetooth.Instance().Send(commandJsonObject.ToString());
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
		
	}

	void stopRotated(){
		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","rotate");
		commandJsonObject.AddField("x",0);
		commandJsonObject.AddField("y",0);
		
		if(Bluetooth.Instance().IsConnected()){
			Bluetooth.Instance().Send(commandJsonObject.ToString());
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
	}



	// Update is called once per frame
	void Update () {
	
	}
}
