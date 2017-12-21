using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelController : MonoBehaviour {

	public GameObject RoundTextObj;
	public GameObject TurnTextObj;

	private Text _roundText;
	private Text _turnText;

	private GameDirector _gd;

	// Use this for initialization
	void Start () {
		_roundText = RoundTextObj.GetComponent<Text> ();
		_turnText = TurnTextObj.GetComponent<Text> ();

		_gd = GameObject.Find("GameDirector").GetComponent<GameDirector>();
	}
	
	// Update is called once per frame
	void Update () {

		_roundText.text = "Round   " +  _gd.RoundCount.ToString() + " / 3";
		_turnText.text  = "Turn     " + _gd.TurnCount.ToString() + " / 10";

	}
}
