using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour {


	public WeaponType weaponType;
	public List<GameObject> weaponRoot;
	public int currentWeaponIndex = 0;

	//Weapon Script
	public BulletShooter bulletShooter;
	public GrenadeScript grenadeThrower;


	public enum WeaponType
	{
		handGun,
		handGrenade
	}

	public void SwitchWeapon()
	{
		if (weaponType == WeaponType.handGrenade)
		{
			if(grenadeThrower.ready)
			{
			weaponRoot [currentWeaponIndex].SetActive (false);
			}
		}
		else
		{
			weaponRoot [currentWeaponIndex].SetActive (false);
		}

		currentWeaponIndex = (currentWeaponIndex + 1) % weaponRoot.Count;

		weaponRoot [currentWeaponIndex].SetActive (true);
		weaponType = (WeaponType)currentWeaponIndex;
	}

	public void TriggerAttack()
	{
		switch (weaponType) 
		{
		case WeaponType.handGun:
			bulletShooter.singleFire = true;
			break;
		case WeaponType.handGrenade:
			grenadeThrower.throwGrenade();
			break;
		}
	}

}
