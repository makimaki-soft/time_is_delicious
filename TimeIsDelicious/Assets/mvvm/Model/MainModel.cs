using RuleManager;
using System.Collections.ObjectModel;

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
        NotStarted,
        RoundInit,
        Betting,
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

    public void StartTimeIsDelicious()
    {
        foreach (var player in _timesIsDelicious.Players)
        {
            _players.Add(player);
        }
    }

    // ラウンドを開始。
    public void StartTimeIsDeliciousRound()
    {
        CurrentStatus = Status.RoundInit;
        var cards = _timesIsDelicious.StartRound();
        foreach(var card in cards)
        {
            _currentFoodCards.Add(card);
        }
        CurrentStatus = Status.Betting;
        CurrentPlayer = _players[0];
    }

    public void AdvanceTime(int i)
    {
        _timesIsDelicious.AdvanceTime(i);
    }

    public void SellCurrentPlayersFood()
    {
        CurrentPlayer.Sell(_currentFoodCards[0]); // デバッグ用実装
    }

    public void BetFood()
    {
        CurrentPlayer.Bet(_currentFoodCards[0]); // デバッグ用実装
    }

}
