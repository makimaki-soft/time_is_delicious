﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DiceController : MonoBehaviour {

	public GameObject spawnPoint;
	public UnityAction StopDice;

	private GameObject dice;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (!Dice.rolling) {
			if (StopDice != null) {
				Debug.Log (Dice.Value (""));
				StopDice ();
			} 
		}
	}

	// dertermine random rolling force
	private Vector3 Force() {
		Vector3 rollTarget = Vector3.zero + new Vector3(2 + 7 * Random.value, .5F + 4 * Random.value, -2 - 3 * Random.value);
		return Vector3.Lerp(spawnPoint.transform.position, rollTarget, 1).normalized * (-35 - Random.value * 20);
	}

	/*
	 * color: "red", "green", "blue", "yellow", "white", "black"
	 */
	public void Roll (string color) {
		Dice.Roll("1d6", "d6-red", spawnPoint.transform.position, Force());
	}
		
}
