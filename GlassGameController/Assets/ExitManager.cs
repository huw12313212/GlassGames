using UnityEngine;
using System.Collections;

public class ExitManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public Camera GUICamera;
	public CommunicationManager communicationManager;
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) 
		{
			Vector3 pos = GUICamera.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hitInfo = Physics2D.Raycast(pos, Vector2.zero);


			if(hitInfo)
			{
				if(hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Exit"))
				{
					Debug.Log("exit");

					if(communicationManager.Playable)
					{
						JSONObject json = new JSONObject ();
						json.AddField ("command", "exit");
						communicationManager.SendJson (json);
					}
				}
			}
		}

		Touch[] touchs = Input.touches;

		foreach (Touch touch in touchs)
		{
			if(touch.phase== TouchPhase.Began)
			{

				Vector3 pos = GUICamera.ScreenToWorldPoint(touch.position);
				RaycastHit2D hitInfo = Physics2D.Raycast(pos, Vector2.zero);
				
				
				if(hitInfo)
				{
					if(hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Exit"))
					{
						Debug.Log("exit");
						
						if(communicationManager.Playable)
						{
							JSONObject json = new JSONObject ();
							json.AddField ("command", "exit");
							communicationManager.SendJson (json);
						}
					}
				}
			}
		}
	}
}
