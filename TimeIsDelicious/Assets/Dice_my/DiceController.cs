using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class DiceController : MonoBehaviour {

	public GameObject spawnPoint;
	public UnityAction StopDice;
	public GameObject diceBtnObj;

	private GameObject dice;
	private bool _stopedDice;

	private GameDirector _gd;

	// Use this for initialization
	void Start () {
		_gd = GameObject.Find("GameDirector").GetComponent<GameDirector>();
		diceBtnObj.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (!Dice.rolling && !_stopedDice) {
			_stopedDice = true;
			Debug.Log ("Dice is :" + _stopedDice);
			if (StopDice != null) {
				Debug.Log (Dice.Value (""));
				StopDice ();
			} 
		}
			
		if (_gd.Status == GameDirectorVM.Status.CastDice) {
			diceBtnObj.SetActive (true);
		} else {
			diceBtnObj.SetActive (false);
		}
	}

	// dertermine random rolling force
	private Vector3 Force() {
		//Vector3 rollTarget = Vector3.zero + new Vector3(2 + 7 * Random.value, .5F + 4 * Random.value, -2 - 3 * Random.value);
		//return Vector3.Lerp(spawnPoint.transform.position, rollTarget, 1).normalized * (-35 - Random.value * 20);
		Vector3 rollTarget = Vector3.zero + new Vector3(100 * Random.value, 100 * Random.value, 100 * Random.value);
		return spawnPoint.transform.forward * 750;
	}

	/*
	 * color: "red", "green", "blue", "yellow", "white", "black"
	 */
	public void Roll (string color) {
		_stopedDice = false;
		Dice.Roll("1d6", "d6-red", spawnPoint.transform.position, Force());
	}
		
}
