using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;
public class GGestureManager : MonoBehaviour {


	public GameObject TouchPanel;
	public CommunicationManager communicationManager;
	
	// Use this for initialization
	void Start () {
		//IGestureManager

		TouchManager.Instance.TouchesBegan += touchBegin;

		TapGesture[] taps = TouchPanel.GetComponents<TapGesture> ();

		foreach (TapGesture g in taps) 
		{
			if(g.NumberOfTapsRequired == 1)
			{
				g.Tapped += (object sender, System.EventArgs e) => 
				{
					singleTap();
				};
			}
			else if(g.NumberOfTapsRequired == 2)
			{
				g.Tapped += (object sender, System.EventArgs e) => 
				{
					doubleTap();
				};
			}
		}

		/*
		singleTapGesture.Tapped += (object sender, System.EventArgs e) => 
		{
			Debug.Log("single tap");
		};*/

	}

	
	public void singleTap()
	{
		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","singleTap");
		
		communicationManager.SendJson (commandJsonObject);
	}
	
	public void doubleTap()
	{
		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","doubleTap");
		
		
		communicationManager.SendJson (commandJsonObject);
	}

	private void touchBegin(object sender, TouchEventArgs e)
	{
		//Debug.Log(e.Touches[0].Position);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
