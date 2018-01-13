using RuleManager;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using UniRx;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

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

    public IReactiveProperty<int> TurnCount { get; private set; }
    public IReactiveProperty<int> RoundCount { get; private set; }
    public IReactiveProperty<Status> CurrentStatus { get; private set; }
    public IReactiveProperty<Player> CurrentPlayer { get; private set; }

    private Subject<Unit> onInitialized = new Subject<Unit>();
    private Subject<Unit> onRoundInitialized = new Subject<Unit>();
    private Subject<Unit> onDiceCasted = new Subject<Unit>();
    private Subject<Unit> onEventOpened = new Subject<Unit>();
    private Subject<Unit> onAged = new Subject<Unit>();
    private Subject<Unit> onPassed = new Subject<Unit>();

    TimeIsDelicious timesIsDelicious;
    int NumberOfPlayers
    {
        get { return timesIsDelicious.NumberOfPlayers; }
    }

    public MainModel(TimeIsDelicious ruleManager)
    {
        timesIsDelicious = ruleManager;

        TurnCount = new ReactiveProperty<int>(0);
        RoundCount = new ReactiveProperty<int>(0);
        CurrentPlayer = new ReactiveProperty<Player>();
        CurrentStatus = new ReactiveProperty<Status>(Status.NotStarted);
    }

    IEnumerator PhaseControl()
    {
        TurnCount.Value = 0;
        RoundCount.Value = 0;

        CurrentStatus.Value = Status.Init;

        // UIの初期化完了を待ち合わせ
        yield return onInitialized.First().ToYieldInstruction();

        // ラウンド開始
        for (RoundCount.Value = 1; RoundCount.Value <= 3; RoundCount.Value++)
        {
            MakiMaki.Logger.Info("PhaseCoroutine : WaitForRoundStart");

            CurrentStatus.Value = Status.WaitForRoundStart; // ラウンド開始待ちに移行

            // UIの初期化完了を待ち合わせ
            yield return onRoundInitialized.First().ToYieldInstruction();

            CurrentStatus.Value = Status.Betting; // 賭けフェイズに移行

            for (int bets = 0; bets < 2; bets++)
            {
                MakiMaki.Logger.Info("Bets : " + (bets + 1).ToString() + "個め");
                for (int i = 0; i < NumberOfPlayers; i++)
                {
                    CurrentPlayer.Value = timesIsDelicious.Players[i];    // 最初のプレイヤーに設定

                    yield return CurrentPlayer.Value.Bets.ObserveCountChanged(true)
                                              .Where(cnt => cnt == (bets + 1))
                                              .First()
                                              .ToYieldInstruction();
                }
            }

            for (TurnCount.Value = 1; TurnCount.Value <= 10; TurnCount.Value++)
            {
                CurrentStatus.Value = Status.CastDice; // ダイスに移行

                // DiceCastedの通知を待ち受け
                yield return onDiceCasted.First().ToYieldInstruction();

                CurrentStatus.Value = Status.DecisionMaking; // 売るかどうかの選択に移行

                for (int i = 0; i < NumberOfPlayers; i++)
                {
                    CurrentPlayer.Value = timesIsDelicious.Players[i];    // 最初のプレイヤーに設定

                    yield return Observable.Amb(
                        CurrentPlayer.Value.Bets.ObserveCountChanged(true).Where(cnt => cnt == 0).AsUnitObservable(),
                        onPassed
                    ).First().ToYieldInstruction();
                }

                CurrentStatus.Value = Status.Event; // イベントに移行

                // EventCardの更新を待ち合わせ
                yield return onEventOpened.First().ToYieldInstruction();

                CurrentStatus.Value = Status.Aging; // 熟成待ちに移行

                yield return onAged.First().ToYieldInstruction();
                yield return new WaitForSeconds(2);

                var NotRotten = timesIsDelicious.RoundFoodCard.Where(fc => fc.Rotten.Value == false)
                                                 .Select(fc => fc.GUID)
                                                 .Count();
                var StillHave = timesIsDelicious.Players.Where(player => player.Bets.Count > 0)
                                   .Select(player => player.GUID)
                                   .Count();

                if (NotRotten == 0 || StillHave == 0)
                {
                    break;
                }
            }

            // 次のラウンドに行く前に、持っているカードはすべて売る
            // この処理は、「ラウンド修了」などのステータスを作ってPresenter側でやるべき
            foreach (var player in timesIsDelicious.Players)
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
    public void StartTimeIsDelicious()
    {
        onInitialized.OnNext(Unit.Default);
    }

    // ラウンドを開始
    public void NotifyRoundInitialized()
    {
        onRoundInitialized.OnNext(Unit.Default);
    }

    public void NotifyAged()
    {
        onAged.OnNext(Unit.Default);
    }

    public void Pass()
    {
        onPassed.OnNext(Unit.Default);
    }

    public void NotifyEventCardChecked()
    {
        onEventOpened.OnNext(Unit.Default);
    }

    // さいころを振ったことを通知(ステータスを変えるだけ)
    public void NotifyDiceCasted()
    {
        onDiceCasted.OnNext(Unit.Default);
    }
}
