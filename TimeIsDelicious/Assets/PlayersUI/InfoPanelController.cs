using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelController : MonoBehaviour {

	public GameObject RoundTextObj;
	public GameObject TurnTextObj;

	private Text _roundText;
	private Text _turnText;

	// Use this for initialization
	void Start () {
		_roundText = RoundTextObj.GetComponent<Text> ();
		_turnText = TurnTextObj.GetComponent<Text> ();
	}

    public void UpdateRound(int RoundCount)
    {
        _roundText.text = "Round   " + RoundCount.ToString() + " / 3";
    }

    public void UpdateTurn(int TurnCount)
    {
        _turnText.text = "Turn     " + TurnCount.ToString() + " / 10";
    }
}
