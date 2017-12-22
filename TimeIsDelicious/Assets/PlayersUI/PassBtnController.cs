using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassBtnController : MonoBehaviour {

	public GameObject passBtnObj;

	private GameDirector _gd;

	// Use this for initialization
	void Start () {
		_gd = GameObject.Find("GameDirector").GetComponent<GameDirector>();

		passBtnObj.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (_gd.Status == GameDirectorVM.Status.DecisionMaking) {
			passBtnObj.SetActive (true);
		} else {
			passBtnObj.SetActive (false);
		}
	}

}
