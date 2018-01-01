using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class ScoreExtension
{
    public static PermanentObj.PlayerScore ToPlayerScore(this RuleManager.Player playerModel)
    {
        var ret = new PermanentObj.PlayerScore();
        ret.ID = playerModel.ID;
        ret.Name = playerModel.Name;
        ret.TotalEarned = playerModel.TotalEarned.Value;
        return ret;
    }
}

public class PermanentObj : MonoBehaviour {

    public class PlayerScore
    {
        public int ID;
        public string Name;
        public int TotalEarned;
    }

	// player num
	private int _playerNum;
	public int playerNum {
		set { _playerNum = value; }
		get { return _playerNum; }
	}
		
	// players info for GameEnd Secne
    private PlayerScore[] _players;
    public PlayerScore[] players {
		set { _players = value; }
		get { return _players; }
	}

	void Awake() {
		DontDestroyOnLoad (this);
	}
}
