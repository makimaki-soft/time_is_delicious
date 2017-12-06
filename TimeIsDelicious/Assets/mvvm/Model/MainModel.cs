using RuleManager;
using System.Collections.ObjectModel;

// シングルトンによる共有インスタンスの実現
public sealed class MainModel {

    private static MainModel instance = new MainModel();

    public static MainModel Instance
    {
        get
        {
            return instance;
        }
    }

    private TimeIsDelicious _timesIsDelicious;
    private Player _currentPlayer;
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
    }

    public void StartTimeIsDelicious()
    {
        foreach ( var player in _timesIsDelicious.Players )
        {
            _players.Add(player);
        }
    }

    // ラウンドを開始。
    public void StartTimeIsDeliciousRound()
    {
        var cards = _timesIsDelicious.StartRound();
        foreach(var card in cards)
        {
            _currentFoodCards.Add(card);
        }
        _currentPlayer = _players[0];
    }

    public void AdvanceTime(int i)
    {
        _timesIsDelicious.AdvanceTime(i);
    }

    public void SellCurrentPlayersFood()
    {
        _currentPlayer.Sell(_currentFoodCards[0]); // デバッグ用実装
    }

    public void BetFood()
    {
        _currentPlayer.Bet(_currentFoodCards[0]); // デバッグ用実装
    }

}
