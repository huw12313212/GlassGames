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
	public DynamicCrossHairController CrossHairController;
	public KnifeController knifeController;


	public enum WeaponType
	{
		handGun,
		handGrenade,
		knife,
		light,
	}

	public void SwitchWeapon()
	{

		weaponRoot [currentWeaponIndex].SetActive (false);

		currentWeaponIndex = (currentWeaponIndex + 1) % weaponRoot.Count;
		weaponType = (WeaponType)currentWeaponIndex;

		if (weaponType == WeaponType.handGrenade)
		{
			CrossHairController.gameObject.SetActive(false);
		}
		else if(weaponType == WeaponType.handGun)
		{
			CrossHairController.gameObject.SetActive(true);
			CrossHairController.Reset();
			bulletShooter.weaponType = BulletShooter.WeaponType.gun;
		}
		else if(weaponType == WeaponType.handGun)
		{
			CrossHairController.gameObject.SetActive(false);
		}
		else if(weaponType == WeaponType.light)
		{
			CrossHairController.gameObject.SetActive(true);
			CrossHairController.Reset();
			bulletShooter.weaponType = BulletShooter.WeaponType.flashLight;
		}

		weaponRoot[currentWeaponIndex].SetActive(true);

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

		case WeaponType.knife:
			knifeController.Attack();
			break;
			
		}
	}

}
