using RuleManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using UnityEngine;
using UniRx;

public class PlayerVM {
    
    public int ID { get; private set; }
    public string Name
    {
        get { return _playerModel.Name; }
    }

    private Player _playerModel;
    public PlayerVM(Player model)
    {
        this.ID = model.ID;
        _playerModel = model;
        _playerModel.Bets.ObserveRemove().Subscribe((item)=>
        {
            var card = item.Value;
            if(card.Rotten.Value)
            {
                // 腐ったことにより手放した
                playerUI.Sadden();
            }
        });
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
