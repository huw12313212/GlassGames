﻿using UnityEngine;
using System.Collections;
using TouchScript;
using System.Collections.Generic;
using TouchScript.Gestures;
using Holoville.HOTween;
public class GGestureManager : MonoBehaviour {


	public GameObject TouchPanel;
	public FlickGesture flickGesture;
	public CommunicationManager communicationManager;
	public TapGesture RightArrow;
	public TapGesture LeftArrow;

	public GameObject weaponBase;
	public List<GameObject> allWeaponIcon;
	public bool WeaponChanging = false;

	public float ShiftDistance;
	public float AnimationTime;
	public float ScaleRatio;

	public EaseType easeType;



	public enum WeaponType
	{
		gun,
		grenade,
		knife,
		flashLight,
		Count,
		car,
	}

	private WeaponType _weapon = WeaponType.gun;
	public WeaponType Weapon 
	{
		get
		{
			return _weapon;
		}
	}

	// Use this for initialization
	void Start () {
		//IGestureManager

		HOTween.Init (true,true,true);

		for (int i =0;i<allWeaponIcon.Count;i++)
		{
			GameObject o = allWeaponIcon[i];

			if(i == (int)_weapon)continue;
			if(i == (int)WeaponType.Count)break;

			o.transform.localScale = o.transform.localScale / ScaleRatio;
		}

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


		flickGesture.Flicked += (object sender, System.EventArgs e) => 
		{

			Debug.Log("flicked:"+flickGesture.ScreenFlickVector);

			//flickGesture.scree

			if(flickGesture.ScreenFlickVector.x>0)
			{
				changeToPreviousWeapon();
			}
			else if(flickGesture.ScreenFlickVector.x<0)
			{
				changeToNextWeapon();
			}
		};

		RightArrow.Tapped += (object sender, System.EventArgs e) => 
		{
			changeToNextWeapon();
		};

		LeftArrow.Tapped += (object sender, System.EventArgs e) => 
		{
			changeToPreviousWeapon();
		};
	}

	private void changeToNextWeapon()
	{
		if (WeaponChanging)
						return;



		Debug.Log ("changeWeapon : next");

		//no next;
		if (((int)_weapon) == ((int)WeaponType.Count-1))return;

		WeaponChanging = true;

		HOTween.To(weaponBase.transform,AnimationTime,new TweenParms()
		           .Prop("position",new Vector3(-ShiftDistance,0,0),true)
		           .Ease(easeType).OnStepComplete(weaponChangingDone));

		HOTween.To(allWeaponIcon[(int)_weapon].transform,AnimationTime,new TweenParms()
		           .Prop("localScale",allWeaponIcon[(int)_weapon].transform.localScale/ScaleRatio));

		_weapon++;

		HOTween.To(allWeaponIcon[(int)_weapon].transform,AnimationTime,new TweenParms()
		           .Prop("localScale",allWeaponIcon[(int)_weapon].transform.localScale*ScaleRatio));

		CheckWeaponState();

		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","changeWeapon");
		commandJsonObject.AddField("direction","next");
		
		communicationManager.SendJson (commandJsonObject);
	}

	private void changeToPreviousWeapon()
	{
		if (WeaponChanging)
						return;


		Debug.Log ("changeWeapon : previous");

		//no previous;
		if (((int)_weapon) == 0)return;

		WeaponChanging = true;

		HOTween.To(weaponBase.transform,AnimationTime,new TweenParms()
		           .Prop("position",new Vector3(ShiftDistance,0,0),true)
		           .Ease(easeType).OnStepComplete(weaponChangingDone));



		HOTween.To(allWeaponIcon[(int)_weapon].transform,AnimationTime,new TweenParms()
		           .Prop("localScale",allWeaponIcon[(int)_weapon].transform.localScale/ScaleRatio));


		_weapon--;

		HOTween.To(allWeaponIcon[(int)_weapon].transform,AnimationTime,new TweenParms()
		           .Prop("localScale",allWeaponIcon[(int)_weapon].transform.localScale*ScaleRatio));


		CheckWeaponState();

		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","changeWeapon");
		commandJsonObject.AddField("direction","previous");
		
		communicationManager.SendJson (commandJsonObject);
	}

	public void CheckWeaponState()
	{
		if (((int)_weapon) == 0)
			LeftArrow.gameObject.SetActive (false);
	
		if (((int)_weapon) == 1)
			LeftArrow.gameObject.SetActive (true);

		if (((int)_weapon) == ((int)WeaponType.Count - 1))
			RightArrow.gameObject.SetActive (false);

		if (((int)_weapon) == ((int)WeaponType.Count - 2))
			RightArrow.gameObject.SetActive (true);

	}
	
	public void singleTap()
	{
		if (WeaponChanging)
						return;

		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","singleTap");
		
		communicationManager.SendJson (commandJsonObject);
	}
	
	public void doubleTap()
	{
		if (WeaponChanging)
			return;

		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","doubleTap");
		
		
		communicationManager.SendJson (commandJsonObject);
	}

	public void weaponChangingDone()
	{
		WeaponChanging = false;
	}

	private void touchBegin(object sender, TouchEventArgs e)
	{
		//Debug.Log(e.Touches[0].Position);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
