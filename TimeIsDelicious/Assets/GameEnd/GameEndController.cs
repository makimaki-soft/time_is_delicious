﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ReturnToStart () {
		Debug.Log ("return to start");
		Application.LoadLevel ("Main");
	}
}