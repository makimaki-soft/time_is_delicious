using RuleManager;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerVM : VMBase {

    private readonly int _id;
    public int ID
    {
        get { return _id; }
    }

    private int _totalEarned;
    public int TotalEarned
    {
        get { return _totalEarned; }
        private set
        {
            if (value != _totalEarned)
            {
                _totalEarned = value;
                NotifyPropertyChanged();
            }
        }
    }

    private Player _playerModel;
    public PlayerVM(Player model)
    {
        _id = model.ID;
        _totalEarned = model.TotalEarned;
        _playerModel = model;
        _playerModel.PropertyChanged += _playerModel_PropertyChanged;
    }

    private void _playerModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var player = (Player)sender;
        switch(e.PropertyName)
        {
            case "TotalEarned":
                TotalEarned = player.TotalEarned;
                break;
        }
    }
}
