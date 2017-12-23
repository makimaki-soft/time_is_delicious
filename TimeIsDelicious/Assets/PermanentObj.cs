using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentObj : MonoBehaviour {

	// player num
	private int _playerNum;
	public int playerNum {
		set { _playerNum = value; }
		get { return _playerNum; }
	}
		
	// players info for GameEnd Secne
	private PlayerVM[] _players;
	public PlayerVM[] players {
		set { _players = value; }
		get { return _players; }
	}

	void Awake() {
		DontDestroyOnLoad (this);
	}
}
