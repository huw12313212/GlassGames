using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AimTypeController : MonoBehaviour {

	public CommunicationManager communicationManager;
	public GyroManager gyroManager;

	public bool GyroListening;
	public float GunYValue = -0.8f;
	public float ViewPortZValue = -0.8f;

	public List<GameObject> ViewPortCenterEnables = new List<GameObject>();
	public List<GameObject> GunAimEnables = new List<GameObject>();

	public delegate void AimTypeChangeEventHandler(AimType type);
	public event AimTypeChangeEventHandler AimTypeChangeEvent;

	public enum AimType
	{
		viewportCenter,
		phoneGun,
	}

	private AimType _aimType = AimType.viewportCenter;

	//public voi


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

		if (AimTypeChangeEvent != null)
		{
			AimTypeChangeEvent(_aimType);
		}

		JSONObject json = new JSONObject();
		json.AddField("command","changeAimMode");
		json.AddField("mode","phoneGun");

		SetEnableList (GunAimEnables,true);
		SetEnableList (ViewPortCenterEnables, false);

		communicationManager.SendJson(json);
	}

	public void changeToViewPortMode()
	{
		gyroManager.Transfering = false;
		
		_aimType = AimType.viewportCenter;

		if (AimTypeChangeEvent != null)
		{
			AimTypeChangeEvent(_aimType);
		}
		
		JSONObject json = new JSONObject();
		json.AddField("command","changeAimMode");
		json.AddField("mode","viewportCenter");

		SetEnableList (GunAimEnables,false);
		SetEnableList (ViewPortCenterEnables, true);
		
		communicationManager.SendJson(json);
	}

	public void SetEnableList(List<GameObject> list,bool active){

		foreach (GameObject o in list) 
		{
			o.SetActive(active);
		}
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
