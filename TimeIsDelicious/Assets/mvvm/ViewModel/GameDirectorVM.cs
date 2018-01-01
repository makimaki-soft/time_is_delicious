using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UniRx;

public class GameDirectorVM : VMBase {

    //todo : Modelと全く同じ定義。本来VMで持つべき？
    public enum Status
    {
        NotStarted,         // スタート待ち
        WaitForRoundStart,  // ラウンド開始待ち
        Betting,            // 賭け中
        // ループ
        CastDice,           // サイコロ待ち
        DecisionMaking,     // 売る・熟成判断待ち
        Event,              // イベントカードオープン待ち
        Aging,              // 熟成待ち
        NextTurn,           // 次のターンへの移行待ち
        // ループ終わり
        GameEnd             // ゲーム終了
    }

    private MainModel _singletonMainModel;

    public GameDirectorVM()
    {
        _singletonMainModel = MainModel.Instance;
        // _singletonMainModel.PropertyChanged += MainModel_PropertyChanged;

        _singletonMainModel.CurrentStatus.Subscribe(status=>
        {
            CurrentStatus = (Status)Enum.ToObject(typeof(Status), (int)status);
        });

        _singletonMainModel.CurrentPlayer.Subscribe(player=>
        {
            if(player == null)
            {
                return;
            }

            if (currentPlayerBetsDisposable != null)
            {
                currentPlayerBetsDisposable.Dispose();
            }

            currentPlayerBetsDisposable = player.Bets.ObserveCountChanged(true).Subscribe(cnt => CurrentPlayersBets = cnt);
            CurrentPlayersBets = player.Bets.Count;
            CurrentPlayerName = player.Name; // tmp
        });

        _singletonMainModel.TurnCount.Subscribe(cnt=>{
            TurnCount = cnt;
        });

        _singletonMainModel.RoundCount.Subscribe(cnt=>
        {
            RoundCount = cnt;
        });
    }

    private IDisposable currentPlayerBetsDisposable;

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

    public string CurrentPlayerNameForce
    {
        set
        {
            _currentPlayerName = value;
            NotifyPropertyChanged("CurrentPlayerName");
        }
    }

    private int _currentPlayersBets;
    public int CurrentPlayersBets
    {
        get { return _currentPlayersBets; }
        set
        {
            if (_currentPlayersBets != value)
            {
                _currentPlayersBets = value;
                NotifyPropertyChanged();
            }
        }
    }

    public void StartTimeIsDelicious(int numOfPlayers)
    {
        _singletonMainModel.StartTimeIsDelicious(numOfPlayers);
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

    public void EventCardOpen()
    {
        _singletonMainModel.EventCardOpen();
    }

    public void GoNextTurn()
    {
        _singletonMainModel.GoNextTurn();
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
