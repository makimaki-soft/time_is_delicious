﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusPanelController : MonoBehaviour {

	public GameObject statusTextObj;

	private Text _statusText;

	// Use this for initialization
	void Start () {
		_statusText = statusTextObj.GetComponent<Text> ();
	}

    public MainModel.Status gdStatus { get; set; }
	
	// Update is called once per frame
	void Update () {

		string msg = "";

        switch(gdStatus) {
		case MainModel.Status.WaitForRoundStart:
			msg = "準備中...";
			break;
		case MainModel.Status.Betting:
			msg = "ベットする肉を決めてください";
			break;
		case MainModel.Status.CastDice:
			msg = "サイコロをふってください";
			break;
		case MainModel.Status.DecisionMaking:
			msg = "売るかもう少し熟成させるか決めてください";
			break;
		case MainModel.Status.Event:
			msg = "熟.....塾々..........塾々々...............";
			break;
		case MainModel.Status.Aging:
			msg = "今日の天気";
			break;
        case MainModel.Status.NextTurn:
            msg = "少し美味しくなりました！";
            break;
        }
        

        _statusText.text = msg;
	}
}
