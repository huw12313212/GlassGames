using UnityEngine;
using System.Collections;

public class AimTypeController : MonoBehaviour {

	public CommunicationManager communicationManager;
	public GyroManager gyroManager;

	public bool GyroListening;
	public float GunYValue = -0.8f;
	public float ViewPortZValue = -0.8f;

	public enum AimType
	{
		viewportCenter,
		phoneGun,
	}

	private AimType _aimType = AimType.viewportCenter;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(communicationManager.Playable)
		{
			if (GyroListening) 
			{
				if(_aimType == AimType.viewportCenter)
				{
					if(gyroManager.lastAcc.y < GunYValue)
					{
						changeToGunMode();
					}
				}
				else if(_aimType == AimType.phoneGun)
				{
					if(gyroManager.lastAcc.z < ViewPortZValue)
					{
						changeToViewPortMode();
					}
				}
			}
		}

	}

	public void changeToGunMode()
	{
		gyroManager.Transfering = true;
		
		_aimType = AimType.phoneGun;
		
		JSONObject json = new JSONObject();
		json.AddField("command","changeAimMode");
		json.AddField("mode","phoneGun");
		
		communicationManager.SendJson(json);
	}

	public void changeToViewPortMode()
	{
		gyroManager.Transfering = false;
		
		_aimType = AimType.viewportCenter;
		
		JSONObject json = new JSONObject();
		json.AddField("command","changeAimMode");
		json.AddField("mode","viewportCenter");
		
		communicationManager.SendJson(json);
	}

	void OnGUI()
	{
		if(communicationManager.Playable)
		{
			if(!GyroListening)
			{
				if (_aimType == AimType.viewportCenter) 
				{
					if (GUI.Button(new Rect(Screen.width-100, 0, 100, 100), "change to gun"))
					{
						changeToGunMode();
					}
						
				} 
				else if (_aimType == AimType.phoneGun) 
				{
					if (GUI.Button(new Rect(Screen.width-100, 0, 100, 100), "change to viewport"))
					{

						changeToViewPortMode();
					}	
				}
			}

		}
	}
}
