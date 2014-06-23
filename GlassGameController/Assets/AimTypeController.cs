using UnityEngine;
using System.Collections;

public class AimTypeController : MonoBehaviour {

	public CommunicationManager communicationManager;
	public GyroManager gyroManager;

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


	}

	void OnGUI()
	{
		if(communicationManager.Playable)
		{
			if (_aimType == AimType.viewportCenter) 
			{
				if (GUI.Button(new Rect(Screen.width-100, 0, 100, 100), "change to gun"))
				{
					gyroManager.gameObject.SetActive(true);

					_aimType = AimType.phoneGun;

					JSONObject json = new JSONObject();
					json.AddField("command","changeAimMode");
					json.AddField("mode","phoneGun");
					
					communicationManager.SendJson(json);
				}
					
			} 
			else if (_aimType == AimType.phoneGun) 
			{
				if (GUI.Button(new Rect(Screen.width-100, 0, 100, 100), "change to viewport"))
				{
					gyroManager.gameObject.SetActive(false);

					_aimType = AimType.viewportCenter;

					JSONObject json = new JSONObject();
					json.AddField("command","changeAimMode");
					json.AddField("mode","viewportCenter");
					
					communicationManager.SendJson(json);

				}	
			}

		}
	}
}
