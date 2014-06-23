using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrenadeBoomChecker : MonoBehaviour {
	//save monster
	public List<MonsterScript> monsterInRage = new List<MonsterScript>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void hurtAllMonsterInRange(int damage){
		//hurt all monster
		foreach (MonsterScript monsterScript in monsterInRage) {
			monsterScript.Hurt(damage);	
		}

		monsterInRage.Clear ();
	}

	//trigger
	void OnTriggerEnter(Collider other) {
		//add to monster list
		monsterInRage.Add (other.gameObject.GetComponent<MonsterScript>());
	}

	void OnTriggerExit(Collider other){
		//remove from monster list
		monsterInRage.Remove (other.gameObject.GetComponent<MonsterScript>());
	}
}
