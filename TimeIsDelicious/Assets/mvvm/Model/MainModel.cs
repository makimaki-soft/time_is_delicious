using RuleManager;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using UniRx;
using System.Collections;
using UnityEngine;

public sealed class MainModel
{
    public enum Status
    {
        NotStarted,         // スタート待ち
        Init,
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

    public TimeIsDelicious _timesIsDelicious;

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

    private bool init = false;
    private bool init2 = false;
    private bool diceCasted = false;
    private bool passed = false;
    private bool eventOpened = false;
    private bool agedDone = false;

    private IEnumerator PhaseControl()
    {
        init = false;
        CurrentStatus.Value = Status.Init;

        yield return new WaitUntil(() => init);

        for (RoundCount.Value = 1; RoundCount.Value <= 3; RoundCount.Value++)
        {
            MakiMaki.Logger.Info("PhaseCoroutine : WaitForRoundStart");
            init2 = false;
            CurrentStatus.Value = Status.WaitForRoundStart; // ラウンド開始待ちに移行

            yield return new WaitUntil(() => init2);

            CurrentStatus.Value = Status.Betting; // 賭けフェイズに移行

            for (int bets = 0; bets < 2; bets++)
            {
                MakiMaki.Logger.Info("Bets : " + (bets + 1).ToString() + "個め");
                for (int i = 0; i < NumberOfPlayers.Value; i++)
                {
                    CurrentPlayer.Value = Players[i];    // 最初のプレイヤーに設定
                    yield return new WaitUntil(() => CurrentPlayer.Value.Bets.Count == (bets + 1));
                }
            }

            for (TurnCount.Value = 1; TurnCount.Value <= 10; TurnCount.Value++)
            {

                diceCasted = false;
                CurrentStatus.Value = Status.CastDice; // ダイスに移行

                yield return new WaitUntil(() => diceCasted);

                CurrentStatus.Value = Status.DecisionMaking; // 売るかどうかの選択に移行

                for (int i = 0; i < NumberOfPlayers.Value; i++)
                {
                    passed = false;
                    CurrentPlayer.Value = Players[i];    // 最初のプレイヤーに設定
                    yield return new WaitUntil(() => CurrentPlayer.Value.Bets.Count == 0 || passed);
                }

                eventOpened = false;
                CurrentStatus.Value = Status.Event; // イベントに移行

                yield return new WaitUntil(() => eventOpened);

                agedDone = false;
                CurrentStatus.Value = Status.Aging; // 熟成待ちに移行

                yield return new WaitUntil(() => agedDone);
                yield return new WaitForSeconds(2);

                var NotRotten = CurrentFoodCards.Where(fc => fc.Rotten.Value == false)
                                                 .Select(fc => fc.GUID)
                                                 .Count();
                var StillHave = Players.Where(player => player.Bets.Count > 0)
                                   .Select(player => player.GUID)
                                   .Count();

                if (NotRotten == 0 || StillHave == 0)
                {
                    break;
                }
            }

            // 次のラウンドに行く前に、持っているカードはすべて売る
            // この処理は、「ラウンド修了」などのステータスを作ってPresenter側でやるべき
            foreach (var player in Players)
            {
                player.SellAll();
            }
        }

        CurrentStatus.Value = Status.GameEnd;
    }

    // 
    public void Start()
    {
        Observable.FromCoroutine(PhaseControl).Subscribe();
    }

    // ゲームスタート
    public void StartTimeIsDelicious(int numOfPlayers)
    {
        this.TurnCount.Value = 0;
        this.RoundCount.Value = 0;
        this.NumberOfPlayers.Value = numOfPlayers;


        init = true;
    }

    // ラウンドを開始
    public void StartTimeIsDeliciousRound()
    {
        init2 = true;
    }

    public void AdvanceTime(int i)
    {
        _timesIsDelicious.AdvanceTime(i, CurrentEventCard.Value);
        agedDone = true;
    }

    public void GoNextTurn()
    {
    }

    public void SellFood(FoodCard card)
    {
        var targetCard = (from foodcard in CurrentFoodCards where foodcard.GUID == card.GUID select foodcard).Single();
        CurrentPlayer.Value.Sell(targetCard);
    }

    public void Pass()
    {
        passed = true;
    }

    // カレントプレイヤーがカードを選択する
    public void BetFood(FoodCard card)
    {
        var targetCard = (from foodcard in CurrentFoodCards where foodcard.GUID == card.GUID select foodcard).Single();
        CurrentPlayer.Value.Bet(targetCard);
    }

    public void EventCardOpen()
    {
        CurrentEventCard.Value = _timesIsDelicious.OpenEventCard();
        eventOpened = true;
    }

    // さいころを振ったことを通知(ステータスを変えるだけ)
    public void NotifyDiceCasted()
    {
        diceCasted = true;
    }
}
