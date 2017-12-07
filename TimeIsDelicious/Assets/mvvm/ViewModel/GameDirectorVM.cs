using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class GameDirectorVM : VMBase {

    public enum Status
    {
        Betting
    }

    private MainModel _singletonMainModel;

    public GameDirectorVM()
    {
        _singletonMainModel = MainModel.Instance;
        _singletonMainModel.PropertyChanged += MainModel_PropertyChanged;
        _singletonMainModel.StartTimeIsDelicious();
    }

    private void MainModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var mainModel = (MainModel)sender;
        switch(e.PropertyName)
        {
            case "CurrentStatus":
                if(mainModel.CurrentStatus == MainModel.Status.Betting)
                {
                    CurrentStatus = Status.Betting;
                }
                break;
            case "CurrentPlayer":
                CurrentPlayerName = mainModel.CurrentPlayer.GUID.ToString(); // tmp
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
        _singletonMainModel.SellCurrentPlayersFood();
    }

    public void BetFood()
    {
        _singletonMainModel.BetFood();
    }
}
