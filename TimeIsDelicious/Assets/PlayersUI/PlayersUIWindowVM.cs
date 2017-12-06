using RuleManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

    private MainModel _singletonMainModel;

    public PlayersUIWindowVM()
    {
        _playerListVM = new ObservableCollection<PlayerVM>();

        _singletonMainModel = MainModel.Instance;
        // CurrentFoodCardsプロパティ全体を公開してしまうかは悩みどころ。
        _singletonMainModel.Players.CollectionChanged += Players_CollectionChanged;
    }

    private ObservableCollection<PlayerVM> _playerListVM;
    public ObservableCollection<PlayerVM> PlayerListVM
    {
        get { return _playerListVM; }
    }

    private void Players_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems)
                {
                    var playerVM = new PlayerVM((Player)item);
                    _playerListVM.Add(playerVM);
                }
                UnityEngine.Debug.Log("CurrentFoodCards Add");
                break;
            case NotifyCollectionChangedAction.Move:
                UnityEngine.Debug.Log("CurrentFoodCards Move");
                break;
            case NotifyCollectionChangedAction.Remove:
                UnityEngine.Debug.Log("CurrentFoodCards Remove");
                break;
            case NotifyCollectionChangedAction.Replace:
                UnityEngine.Debug.Log("CurrentFoodCards Replace");
                break;
            case NotifyCollectionChangedAction.Reset:
                UnityEngine.Debug.Log("CurrentFoodCards Reset");
                break;
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
