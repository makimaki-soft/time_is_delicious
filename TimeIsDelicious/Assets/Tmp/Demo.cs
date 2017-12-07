using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour {

	public GameObject diceController;

	private DiceController dc;

	// Use this for initialization
	void Start () {
		dc = diceController.GetComponent<DiceController> ();
		dc.StopDice = StopDiceHandler;
		dc.Roll ("red");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void StopDiceHandler () {
		Debug.Log ("Stop Dice");
		Dice.Clear ();
		Destroy (dc);
	}
}
