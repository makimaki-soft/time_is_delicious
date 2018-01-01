﻿using RuleManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using UnityEngine;
using UniRx;

public class PlayerVM : VMBase {

    private readonly int _id;
    public int ID
    {
        get { return _id; }
    }

    public enum Status
    {
        StillHave,
        AlreadySold,
        Rotten
    }

    public class FoodCardStatus
    {
        public Guid guid;
        public Status status;
    }

    private ObservableCollection<FoodCardStatus> _bets;
    public ObservableCollection<FoodCardStatus> Bets
    {
        get { return _bets; }
    }

    public string Name
    {
        get { return _playerModel.Name; }
    }

    private Player _playerModel;
    public PlayerVM(Player model)
    {
        _id = model.ID;
        _bets = new ObservableCollection<FoodCardStatus>();
        _playerModel = model;
        _playerModel.Bets.CollectionChanged += Bets_CollectionChanged;
    }

    private void Bets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:

                foreach (var item in e.NewItems)
                {
                    var card = (FoodCard)item;
                    var status = new FoodCardStatus();
                    status.guid = card.GUID;
                    status.status = Status.StillHave;
                    _bets.Add(status);
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems)
                {
                    var card = (FoodCard)item;

                    foreach( var bet in _bets )
                    {
                        if( bet.guid == card.GUID )
                        {
                            bet.status = card.Rotten.Value ? Status.Rotten : Status.AlreadySold;
                            _bets.Remove(bet);
                            break;
                        }
                    }
                }
                break;
        }
    }

    public PlayerUIController playerUI;
    public PlayerUIController PlayerUI
    {
        get { return playerUI; }
        set
        {
            playerUI = value;
            _playerModel.TotalEarned.Subscribe(earned => playerUI.UpdateTotalEarned(earned));
        }
    }
}
