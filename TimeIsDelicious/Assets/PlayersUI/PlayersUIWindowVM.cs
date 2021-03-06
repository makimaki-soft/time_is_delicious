﻿using RuleManager;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

public class PlayersUIWindowVM : VMBase {

    private MainModel _singletonMainModel;

    public PlayersUIWindowVM()
    {
        _playerListVM = new ObservableCollection<PlayerVM>();

        _singletonMainModel = MainModel.Instance;
        // CurrentFoodCardsプロパティ全体を公開してしまうかは悩みどころ。
        _singletonMainModel.Players.CollectionChanged += Players_CollectionChanged;
        _singletonMainModel.PropertyChanged += SingletonMainModel_PropertyChanged;
    }

    public PermanentObj Permanent { get; set; }

    private ObservableCollection<PlayerVM> _playerListVM;
    public ObservableCollection<PlayerVM> PlayerListVM
    {
        get { return _playerListVM; }
    }

    private int _numberOfPlayers;
    public int NumberOfPlayers
    {
        get { return _numberOfPlayers; }
        set
        {
            if (_numberOfPlayers != value)
            {
                _numberOfPlayers = value;
                NotifyPropertyChanged();
            }

        }
    }

    private PlayerVM _currentPlayer;
    public PlayerVM CurrentPlayer
    {
        get { return _currentPlayer; }
        set
        {
            if (_currentPlayer != value)
            {
                _currentPlayer = value;
                NotifyPropertyChanged();
            }
        }
    }

    private void SingletonMainModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var mainModel = sender as MainModel;
        switch (e.PropertyName)
        {
            case "NumberOfPlayers":
                NumberOfPlayers = mainModel.NumberOfPlayers;
                break;
            case "CurrentPlayer":
                CurrentPlayer = _playerListVM.FirstOrDefault(v => v.ID == mainModel.CurrentPlayer.ID);
                break;
            case "CurrentStatus":
                if (mainModel.CurrentStatus == MainModel.Status.GameEnd)
                {
                    if(Permanent != null)
                    {
                        Permanent.playerNum = _playerListVM.Count;
                        Permanent.players = _playerListVM.ToArray();
                    }
                    FadeManager.Instance.LoadScene("GameEnd", 1.0f);
                }
                break;
        }
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
}
