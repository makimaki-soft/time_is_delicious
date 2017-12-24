using RuleManager;
using System;
using System.Collections.ObjectModel;
using System.Linq;

// シングルトンによる共有インスタンスの実現
public sealed class MainModel : GameComponent {

    private static MainModel instance = new MainModel();
    
    public static void Reset()
    {
        instance = new MainModel();
    }

    public static MainModel Instance
    {
        get
        {
            return instance;
        }
    }

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

    private TimeIsDelicious _timesIsDelicious;

    private Status _currentStatus;
    public Status CurrentStatus
    {
        get { return _currentStatus; }
        private set
        {
            if (_currentStatus != value)
            {
                _currentStatus = value;
                NotifyPropertyChanged();
            }
        }
    }

    private Player _currentPlayer;
    public Player CurrentPlayer
    {
        get { return _currentPlayer; }
        private set
        {
            if (_currentPlayer != value)
            {
                _currentPlayer = value;
                NotifyPropertyChanged();
            }
        }
    }

    // 強制イベント通知
    public Player CurrentPlayerForceEvent
    {
        set
        {
            _currentPlayer = value;
            NotifyPropertyChanged("CurrentPlayer");
        }
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

    private EventCard _currentEventCard;
    public EventCard CurrentEventCard
    {
        get { return _currentEventCard; }
        private set
        {
            if (_currentEventCard != value)
            {
                _currentEventCard = value;
                NotifyPropertyChanged();
            }
        }
    }

    private ObservableCollection<FoodCard> _currentFoodCards;
    public ObservableCollection<FoodCard> CurrentFoodCards
    {
        get { return _currentFoodCards;}
    }

    private ObservableCollection<Player> _players;
    public ObservableCollection<Player> Players
    {
        get { return _players; }
    }

    private MainModel()
    {
        _turnCount = 0;
        _roundCount = 0;

        _timesIsDelicious = new TimeIsDelicious();
        _currentFoodCards = new ObservableCollection<FoodCard>();
        _players = new ObservableCollection<Player>();
        CurrentStatus = Status.NotStarted;
    }

    // ゲームスタート
    public void StartTimeIsDelicious()
    {
        TurnCount = 0;
        RoundCount = 0;
        NumberOfPlayers = 4;
        foreach (var player in _timesIsDelicious.Players)
        {
            _players.Add(player);
        }
        CurrentStatus = Status.WaitForRoundStart; // ラウンド開始待ちに移行
    }

    // ラウンドを開始
    private int _currentPlayerIndex;
    private int _betTurnCount;
    public void StartTimeIsDeliciousRound()
    {
        // CurrentStatus = Status.RoundInit;
        _currentPlayerIndex = 0;
        _betTurnCount = 0;
        var cards = _timesIsDelicious.StartRound();

        // リセット
        while(_currentFoodCards.Count>0)
        {
            _currentFoodCards.RemoveAt(0);
        }
        foreach(var card in cards)
        {
            _currentFoodCards.Add(card);
        }
        RoundCount += 1;
        TurnCount = 1;
        CurrentStatus = Status.Betting; // 賭けフェイズに移行
        CurrentPlayer = _players[_currentPlayerIndex];    // 最初のプレイヤーに設定

        _currentEventCard = _timesIsDelicious.OpenEventCard(); // Debug用
    }

    public void AdvanceTime(int i)
    {
        _timesIsDelicious.AdvanceTime(i, _currentEventCard);
        CurrentStatus = Status.NextTurn;
    }

    public void GoNextTurn()
    {
        var NotRotten = _currentFoodCards.Where(fc => fc.Rotten == false)
                                         .Select(fc => fc.GUID)
                                         .Count();
        var StillHave = _players.Where(player => player.Bets.Count > 0)
                           .Select(player => player.GUID)
                           .Count();

        // すべてのカードが腐るか、すべてのカードが売り払われたら次のラウンドに進む
        if (TurnCount == 10 || NotRotten == 0 || StillHave == 0 )
        {
            TurnCount = 0;
            _currentPlayerIndex = 0;
            
            CurrentStatus = RoundCount == 1 ? Status.GameEnd : Status.WaitForRoundStart; // ラウンド開始待ちor終了に移行
        }
        else
        {
            TurnCount += 1;
            CurrentStatus = Status.CastDice;
            _currentPlayerIndex = 0;
            CurrentPlayer = _players[_currentPlayerIndex];    // 最初のプレイヤーに設定
        }
    }

    public void SellFood(FoodCard card)
    {
        var targetCard = (from foodcard in _currentFoodCards where foodcard.GUID == card.GUID select foodcard).Single();
        CurrentPlayer.Sell(targetCard);
    }

    public void Pass()
    {
        _currentPlayerIndex++;
        if (_currentPlayerIndex >= NumberOfPlayers)
        {
            _currentPlayerIndex = 0;
            _betTurnCount++;
            if (_betTurnCount >= 2)
            {
                CurrentStatus = Status.Event; // イベントに移行
                _currentPlayerIndex = 0;
                return;
            }
        }
        CurrentPlayer = _players[_currentPlayerIndex];    // 次のプレイヤーに設定
    }

    // カレントプレイヤーがカードを選択する
    public void BetFood(FoodCard card)
    {
        var targetCard = (from foodcard in _currentFoodCards where foodcard.GUID == card.GUID select foodcard).Single();
        CurrentPlayer.Bet(targetCard);

        _currentPlayerIndex++;
        if(_currentPlayerIndex >= NumberOfPlayers)
        {
            _currentPlayerIndex = 0;
            _betTurnCount++;
            if(_betTurnCount >= 2)
            {
                CurrentStatus = Status.CastDice; // ダイスに移行
                _currentPlayerIndex = 0;
                CurrentPlayer = _players[_currentPlayerIndex];    // 最初のプレイヤーに設定
                return;
            }
        }
        CurrentPlayer = _players[_currentPlayerIndex];    // 次のプレイヤーに設定
    }

    public void EventCardOpen()
    {
        CurrentEventCard = _timesIsDelicious.OpenEventCard();
        CurrentStatus = Status.Aging; // 熟成待ちに移行
    }

    // さいころを振ったことを通知(ステータスを変えるだけ)
    public void NotifyDiceCasted()
    {
        CurrentStatus = Status.DecisionMaking; // 売るかどうかの選択に移行
        _currentPlayerIndex = 0;
        CurrentPlayerForceEvent = _players[_currentPlayerIndex];
    }
}
