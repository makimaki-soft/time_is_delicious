using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using RuleManager;
using System.Linq;
using System;

public class MainPresenter : MonoBehaviour {
    
    [SerializeField]
    private TempManager foodCardFactory;

    [SerializeField]
    private EventCardController eventCardController;

    [SerializeField]
    private GameDirector gameDirector;

    [SerializeField]
    private PopupMessaegController popupMessageController;

    [SerializeField]
    private PlayersUIWindowController playerWindowController;

    private MainModel _singletonMainModel;
    public PermanentObj Permanent { get; private set; }

	// Use this for initialization
	void Start () {
        _singletonMainModel = MainModel.Instance;
        Permanent = GameObject.Find("PermanentObj")?.GetComponent<PermanentObj>();

        _singletonMainModel.CurrentFoodCards.ObserveAdd().Subscribe(item =>
        {
            var foodCardVM = new FoodCardVM((FoodCard)item.Value);
            foodCardFactory.CreateFoodCard(foodCardVM);
        });

        _singletonMainModel.CurrentFoodCards.ObserveRemove().Subscribe(item =>
        {
            var removedItem = item.Value;
            if (removedItem != null)
            {
                removedItem.Reset();
                foodCardFactory.RemoveFoodCard(removedItem);
            }
        });

        _singletonMainModel.CurrentEventCard.Subscribe(card=>
        {
            if(card==null)
            {
                return;
            }

            eventCardController.SetEventValues(
                card.ID,
                card.Name,
                card.Description,
                card.Weather[RuleManager.EventType.Temperature],
                card.Weather[RuleManager.EventType.Humid],
                card.Weather[RuleManager.EventType.Wind]);
        });

      
        _singletonMainModel.CurrentStatus.Subscribe(status =>
        {
            gameDirector.Status = status;
            if (status == MainModel.Status.WaitForRoundStart)
            {
                StartCoroutine(RoundStart());
            }
        });

        _singletonMainModel.CurrentPlayer.Subscribe(player =>
        {
            if (player == null)
            {
                return;
            }

            if (currentPlayerBetsDisposable != null)
            {
                currentPlayerBetsDisposable.Dispose();
            }

            currentPlayerBetsDisposable = player.Bets.ObserveCountChanged(true).Subscribe(cnt =>
            {
                if (_singletonMainModel.CurrentStatus.Value == MainModel.Status.DecisionMaking)
                {
                    // 売るorパス決定ステータスに入ったとき、カレントプレイヤが肉をもってなかったら自動パス
                    if (cnt == 0)
                    {
                        MainModel.Instance.Pass();
                    }
                }
            });

            gameDirector.CurrentPlayerName = player.Name;
            if (_singletonMainModel.CurrentStatus.Value == MainModel.Status.Betting)
            {
                popupMessageController.Popup(player.Name + "さんは肉を選んでください。");
            }
            if (_singletonMainModel.CurrentStatus.Value == MainModel.Status.DecisionMaking)
            {
                // 売るorパス決定ステータスに入ったとき、カレントプレイヤが肉をもってなかったら自動パス
                if (player.Bets.Count == 0)
                {
                    MainModel.Instance.Pass();
                }
            }
        });

        _singletonMainModel.TurnCount.Subscribe(cnt => {
            gameDirector.TurnCount = cnt;
        });

        _singletonMainModel.RoundCount.Subscribe(cnt =>
        {
            gameDirector.RoundCount = cnt;
        });

        gameDirector.OnDiceButtonClickAsObservable.Subscribe(_=>
        {
            gameDirector.DiceRoll().Subscribe(dice=>
            {
                MainModel.Instance.NotifyDiceCasted(); // 判断待ちステータスに進める
                // 熟成待ち状態になったらダイスの出目で熟成
                StartCoroutine(ControlTurnCoroutine(dice));
            });
        });

        initPlayersUIWindowVM();

        int? numOfPlayers = Permanent?.playerNum;
        _singletonMainModel.StartTimeIsDelicious(numOfPlayers.HasValue ? numOfPlayers.Value : 4);
    }
	
    private IDisposable currentPlayerBetsDisposable;

    private IEnumerator RoundStart()
    {
        // PlyerUIWindowが準備完了したら
        yield return new WaitUntil(() =>
        {
            return playerWindowController.UIRready;
        });

        // メッセージを表示して、確認されたらStartRound
        popupMessageController.Popup("ラウンド開始します", () =>
        {
            _singletonMainModel.StartTimeIsDeliciousRound();
        });
    }

    private IEnumerator ControlTurnCoroutine(int dice)
    {
        yield return new WaitUntil(() =>
        {
            return _singletonMainModel.CurrentStatus.Value == MainModel.Status.Event; // イベントカードオープン待ちになったら
        });

        bool EventChecked = false;
        // イベントカードオープン
        _singletonMainModel.EventCardOpen();
        eventCardController.DrawEventCard(
            () =>
            {
                gameDirector.DebugDiceClean();
                EventChecked = true;
            });

        yield return new WaitUntil(() =>
        {
            return EventChecked && _singletonMainModel.CurrentStatus.Value == MainModel.Status.Aging; // 熟成待ち状態になったら
        });

        // 熟成
        _singletonMainModel.AdvanceTime(dice);

        yield return new WaitUntil(() =>
        {
            return EventChecked && _singletonMainModel.CurrentStatus.Value == MainModel.Status.NextTurn; // 次のターンへの移行待ち状態になったら
        });

        // ２秒待って
        yield return new WaitForSeconds(2.0f);
        _singletonMainModel.GoNextTurn(); // 次のターンへ移行
    }

    private void initPlayersUIWindowVM()
    {
        _singletonMainModel.NumberOfPlayers.Subscribe(val =>
        {
            playerWindowController.MaxNumberOfViewList = val;
        });

        _singletonMainModel.CurrentPlayer
                           .Where((arg) => arg != null)
                           .Subscribe(val =>
                           {
                                playerWindowController.SetCurrentPlayer(val.ID);
                           });

        _singletonMainModel.CurrentStatus.Subscribe(val =>
        {
            if (_singletonMainModel.CurrentStatus.Value == MainModel.Status.GameEnd)
            {
                if (Permanent != null)
                {
                    Permanent.playerNum = _singletonMainModel.NumberOfPlayers.Value;
                    Permanent.players = _singletonMainModel.Players.Select(player => player.ToPlayerScore()).ToArray();
                }
                FadeManager.Instance.LoadScene("GameEnd", 1.0f);
            }
        });

        _singletonMainModel.Players.ObserveAdd().Subscribe(item =>
        {
            playerWindowController.AddPlayer(new PlayerVM(item.Value));
        });
    }
}
