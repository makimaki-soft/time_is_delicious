using RuleManager;
using System;
using System.Collections.ObjectModel;
using System.Linq;

// シングルトンによる共有インスタンスの実現
public sealed class MainModel : GameComponent {

    private static MainModel instance = new MainModel();

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
        CastDice,
        DecisionMaking,
        Event,
        Aging,
        // ループ終わり
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
        _timesIsDelicious = new TimeIsDelicious();
        _currentFoodCards = new ObservableCollection<FoodCard>();
        _players = new ObservableCollection<Player>();
        CurrentStatus = Status.NotStarted;
    }

    // ゲームスタート
    public void StartTimeIsDelicious()
    {
        NumberOfPlayers = 4;
        foreach (var player in _timesIsDelicious.Players)
        {
            _players.Add(player);
        }
        CurrentStatus = Status.WaitForRoundStart; // ラウンド開始待ちに移行
    }

    // ラウンドを開始
    private int _currentPlayerIndex;
    private int _turnCount;
    public void StartTimeIsDeliciousRound()
    {
        // CurrentStatus = Status.RoundInit;
        _currentPlayerIndex = 0;
        _turnCount = 0;
        var cards = _timesIsDelicious.StartRound();
        foreach(var card in cards)
        {
            _currentFoodCards.Add(card);
        }
        CurrentStatus = Status.Betting; // 賭けフェイズに移行
        CurrentPlayer = _players[_currentPlayerIndex];    // 最初のプレイヤーに設定

        _currentEventCard = _timesIsDelicious.OpenEventCard(); // Debug用
    }

    public void AdvanceTime(int i)
    {
        _timesIsDelicious.AdvanceTime(i, _currentEventCard);

        CurrentStatus = Status.WaitForRoundStart; // ラウンド開始待ちに移行
    }

    public void SellFood(FoodCard card)
    {
        var targetCard = (from foodcard in _currentFoodCards where foodcard.GUID == card.GUID select foodcard).Single();
        CurrentPlayer.Sell(targetCard);

        _currentPlayerIndex++;
        if (_currentPlayerIndex > NumberOfPlayers)
        {
            _currentPlayerIndex = 0;
            _turnCount++;
            if (_turnCount >= 2)
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
        if(_currentPlayerIndex > NumberOfPlayers)
        {
            _currentPlayerIndex = 0;
            _turnCount++;
            if(_turnCount >= 2)
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
    }

    // さいころを振ったことを通知(ステータスを変えるだけ)
    public void NotifyDiceCasted()
    {
        CurrentStatus = Status.DecisionMaking; // 売るかどうかの選択に移行
        _currentPlayerIndex = 0;
        CurrentPlayer = _players[_currentPlayerIndex];
    }
}
