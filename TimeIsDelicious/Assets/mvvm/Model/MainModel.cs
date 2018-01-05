using RuleManager;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using UniRx;

// シングルトンによる共有インスタンスの実現
public sealed class MainModel
{
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
    public IReactiveProperty<int> TurnCount { get; private set; }

    // ラウンド数
    public IReactiveProperty<int> RoundCount { get; private set; }

    private TimeIsDelicious _timesIsDelicious;

    public IReactiveProperty<Status> CurrentStatus { get; private set; }

    public IReactiveProperty<Player> CurrentPlayer { get; private set; }
    // 強制イベント通知
    public IReactiveProperty<int> NumberOfPlayers { get; private set; }
    public IReactiveProperty<EventCard> CurrentEventCard { get; private set; }
    public IReactiveCollection<FoodCard> CurrentFoodCards { get; private set; }
    public IReactiveCollection<Player> Players { get; private set; }

    public MainModel()
    {
        _timesIsDelicious = new TimeIsDelicious();

        this.TurnCount = new ReactiveProperty<int>(0);
        this.RoundCount = new ReactiveProperty<int>(0);
        this.NumberOfPlayers = new ReactiveProperty<int>(0);
        this.CurrentFoodCards = new ReactiveCollection<FoodCard>();
        this.CurrentEventCard = new ReactiveProperty<EventCard>();
        this.CurrentPlayer = new ReactiveProperty<Player>();
        this.Players = new ReactiveCollection<Player>();
        this.CurrentStatus = new ReactiveProperty<Status>(Status.NotStarted);
    }

    // ゲームスタート
    public void StartTimeIsDelicious(int numOfPlayers)
    {
        this.TurnCount.Value = 0;
        this.RoundCount.Value = 0;
        this.NumberOfPlayers.Value = numOfPlayers;

        _timesIsDelicious.Start(numOfPlayers);
        foreach (var player in _timesIsDelicious.Players)
        {
            this.Players.Add(player);
        }
        CurrentStatus.Value = Status.WaitForRoundStart; // ラウンド開始待ちに移行
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
        while(CurrentFoodCards.Count>0)
        {
            CurrentFoodCards.RemoveAt(0);
        }
        foreach(var card in cards)
        {
            CurrentFoodCards.Add(card);
        }
        RoundCount.Value += 1;
        TurnCount.Value = 1;
        CurrentStatus.Value = Status.Betting; // 賭けフェイズに移行
        CurrentPlayer.Value = Players[_currentPlayerIndex];    // 最初のプレイヤーに設定

        CurrentEventCard.Value = _timesIsDelicious.OpenEventCard(); // Debug用
    }

    public void AdvanceTime(int i)
    {
        _timesIsDelicious.AdvanceTime(i, CurrentEventCard.Value);
        CurrentStatus.Value = Status.NextTurn;
    }

    public void GoNextTurn()
    {
        var NotRotten = CurrentFoodCards.Where(fc => fc.Rotten.Value == false)
                                         .Select(fc => fc.GUID)
                                         .Count();
        var StillHave = Players.Where(player => player.Bets.Count > 0)
                           .Select(player => player.GUID)
                           .Count();

        // すべてのカードが腐るか、すべてのカードが売り払われたら次のラウンドに進む
        if (TurnCount.Value == 10 || NotRotten == 0 || StillHave == 0 )
        {
            TurnCount.Value = 0;
            _currentPlayerIndex = 0;

            // 次のラウンドに行く前に、持っているカードはすべて売る
            foreach(var player in Players)
            {
                player.SellAll();
            }

            CurrentStatus.Value = RoundCount.Value == 3 ? Status.GameEnd : Status.WaitForRoundStart; // ラウンド開始待ちor終了に移行
        }
        else
        {
            TurnCount.Value += 1;
            CurrentStatus.Value = Status.CastDice;
            _currentPlayerIndex = 0;
            CurrentPlayer.Value = Players[_currentPlayerIndex];    // 最初のプレイヤーに設定
        }
    }

    public void SellFood(FoodCard card)
    {
        var targetCard = (from foodcard in CurrentFoodCards where foodcard.GUID == card.GUID select foodcard).Single();
        CurrentPlayer.Value.Sell(targetCard);
    }

    public void Pass()
    {
        _currentPlayerIndex++;
        if (_currentPlayerIndex >= NumberOfPlayers.Value)
        {
            _currentPlayerIndex = 0;
            _betTurnCount++;
            if (_betTurnCount >= 2)
            {
                CurrentStatus.Value = Status.Event; // イベントに移行
                _currentPlayerIndex = 0;
                return;
            }
        }
        CurrentPlayer.Value = Players[_currentPlayerIndex];    // 次のプレイヤーに設定
    }

    // カレントプレイヤーがカードを選択する
    public void BetFood(FoodCard card)
    {
        var targetCard = (from foodcard in CurrentFoodCards where foodcard.GUID == card.GUID select foodcard).Single();
        CurrentPlayer.Value.Bet(targetCard);

        _currentPlayerIndex++;
        if(_currentPlayerIndex >= NumberOfPlayers.Value)
        {
            _currentPlayerIndex = 0;
            _betTurnCount++;
            if(_betTurnCount >= 2)
            {
                CurrentStatus.Value = Status.CastDice; // ダイスに移行
                _currentPlayerIndex = 0;
                CurrentPlayer.Value = Players[_currentPlayerIndex];    // 最初のプレイヤーに設定
                return;
            }
        }
        CurrentPlayer.Value = Players[_currentPlayerIndex];    // 次のプレイヤーに設定
    }

    public void EventCardOpen()
    {
        CurrentEventCard.Value = _timesIsDelicious.OpenEventCard();
        CurrentStatus.Value = Status.Aging; // 熟成待ちに移行
    }

    // さいころを振ったことを通知(ステータスを変えるだけ)
    public void NotifyDiceCasted()
    {
        CurrentStatus.Value = Status.DecisionMaking; // 売るかどうかの選択に移行
        _currentPlayerIndex = 0;
        CurrentPlayer.Value = Players[_currentPlayerIndex];
    }
}
