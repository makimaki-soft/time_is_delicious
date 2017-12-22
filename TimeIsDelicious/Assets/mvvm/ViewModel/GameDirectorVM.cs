﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

public class GameDirectorVM : VMBase {

    //todo : Modelと全く同じ定義。本来VMで持つべき？
    public enum Status
    {
        NotStarted,         // スタート待ち
        WaitForRoundStart,  // ラウンド開始待ち
        Betting,            // 
        // ループ
        CastDice,
        DecisionMaking,
        Event,
        Aging,
        // ループ終わり
    }

    private MainModel _singletonMainModel;

    public GameDirectorVM()
    {
        _singletonMainModel = MainModel.Instance;
        _singletonMainModel.PropertyChanged += MainModel_PropertyChanged;
    }

    private void MainModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var mainModel = (MainModel)sender;
        switch(e.PropertyName)
        {
            case "CurrentStatus":
                // ステータスを同期
                CurrentStatus = (Status)Enum.ToObject(typeof(Status), (int)mainModel.CurrentStatus);
                UnityEngine.Debug.Log(CurrentStatus);
                break;
            case "CurrentPlayer":
                CurrentPlayerName = mainModel.CurrentPlayer.Name; // tmp
                break;
            case "TurnCount":
                TurnCount = mainModel.TurnCount;
                break;
            case "RoundCount":
                RoundCount = mainModel.RoundCount;
                break;
        }
    }

    private Status _currentStatus;
    public Status CurrentStatus
    {
        get { return _currentStatus; }
        set
        {
            if(_currentStatus != value)
            {
                _currentStatus = value;
                NotifyPropertyChanged();
            }
        }
    }

    // ターン数
    private int _turnCount;
    public int TurnCount
    {
        get { return _turnCount; }
        private set
        {
            if (_turnCount != value)
            {
                _turnCount = value;
                NotifyPropertyChanged();
            }
        }
    }

    // ラウンド数
    private int _roundCount;
    public int RoundCount
    {
        get { return _roundCount; }
        private set
        {
            if (_roundCount != value)
            {
                _roundCount = value;
                NotifyPropertyChanged();
            }
        }
    }

    private string _currentPlayerName;
    public string CurrentPlayerName
    {
        get { return _currentPlayerName; }
        set
        {
            if(_currentPlayerName != value)
            {
                _currentPlayerName = value;
                NotifyPropertyChanged();
            }
        }
    }

    public void StartTimeIsDelicious()
    {
        _singletonMainModel.StartTimeIsDelicious();
    }
    public void StartRound()
    {
        _singletonMainModel.StartTimeIsDeliciousRound();
    }

    public void AdvanceTime(int i)
    {
        // todo エラーチェック
        _singletonMainModel.AdvanceTime(i);
    }

    public void SellFood()
    {
        // _singletonMainModel.SellCurrentPlayersFood();
    }

    public void BetFood()
    {
        // _singletonMainModel.BetFood(null);
    }

	public void NotifyDiceCasted()
	{
		_singletonMainModel.NotifyDiceCasted ();
	}
}
