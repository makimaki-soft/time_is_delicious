using RuleManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersUIWindowVM : VMBase {

    private List<Player> _players;

    public PlayersUIWindowVM(List<Player> players )
    {
        _players = players;
        _score = new Dictionary<Guid, int>(_players.Count);
        foreach (var player in _players)
        {
            _score[player.GUID] = 0;
            player.PropertyChanged += Player_PropertyChanged;
        }
    }

    private void Player_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var player = sender as Player;
        if(e.PropertyName == "TotalEarned")
        {
            Score[player.GUID] = player.TotalEarned;
            Score = Score;
        }
    }

    private Dictionary<Guid, int> _score;
    public Dictionary<Guid, int> Score
    {
        set
        {
            // todo : Valueが変わってもオブジェクトが同じなので発火しない
            //if(!_score.Equals(value)) 
            //{
                _score = value;
                NotifyPropertyChanged();
            //}
        }
        get { return _score; }
    }

}
