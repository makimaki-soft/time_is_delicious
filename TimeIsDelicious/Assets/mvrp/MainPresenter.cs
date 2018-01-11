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

    [SerializeField]
    private InfoPanelController infoPanelController;

    [SerializeField]
    private CardDetailPanelController cardDetailPanel;

    [SerializeField]
    private PassBtnController passButton;

    [SerializeField]
    private DiceController diceController;

    [SerializeField]
    private StatusPanelController statusPanel;

    private List<CardViewModel> cardViewList = new List<CardViewModel>();

    private CompositeDisposable phaseRangedDisposable = new CompositeDisposable();

    private MainModel mainModel;
    public PermanentObj Permanent { get; private set; }

    private TimeIsDelicious ruleManager;

    IDisposable AgedDisposable;
    IDisposable PriceDisposable;
    IDisposable RottenDisposable;

    void onStatusChanged(MainModel.Status status)
    {
        phaseRangedDisposable.Clear();

        // Common
        statusPanel.gdStatus = status;
        diceController.IsActive = false;
        passButton.SetActive(false);

        // Per Status
        switch(status)
        {
            case MainModel.Status.NotStarted:
                break;
            case MainModel.Status.Init:
                onGameStart();
                break;
            case MainModel.Status.WaitForRoundStart:  // ラウンド開始待ち
                onRoundStart();
                break;
            case MainModel.Status.Betting:
                break;// 賭け中
            case MainModel.Status.CastDice:           // サイコロ待ち
                onCastDice();
                break;
            case MainModel.Status.DecisionMaking:     // 売る・熟成判断待ち
                onDecisionMaking();
                break;
            case MainModel.Status.Event:              // イベントカードオープン待ち
                break;
            case MainModel.Status.Aging:              // 熟成待ち
                break;
            case MainModel.Status.NextTurn:           // 次のターンへの移行待ち
                break;
            case MainModel.Status.GameEnd:             // ゲーム終了
                onGameEnd();
                break;
        }
    }

    void onGameStart()
    {
        int? numOfPlayers = Permanent?.playerNum;
        int nop = numOfPlayers.HasValue ? numOfPlayers.Value : 4;

        playerWindowController.MaxNumberOfViewList = nop;

        ruleManager.Start(nop);

        foreach (var player in ruleManager.Players)
        {
            onPlayer(player);
        }

        // debug //
        foreach (var player in ruleManager.Players)
        {
            mainModel.Players.Add(player);
        }

        mainModel.StartTimeIsDelicious(nop);
    }

    void onRoundStart()
    {
        StartCoroutine(RoundStart());
    }

    void onCastDice()
    {
        diceController.IsActive = true;
                      
        gameDirector.OnDiceButtonClickAsObservable.Subscribe(_ =>
        {
            gameDirector.DiceRoll().First().Subscribe(dice =>
            {
                mainModel.NotifyDiceCasted(); // 判断待ちステータスに進める
                // 熟成待ち状態になったらダイスの出目で熟成
                StartCoroutine(ControlTurnCoroutine(dice));
            });
        }).AddTo(phaseRangedDisposable);
    }

    void onDecisionMaking()
    {
        passButton.OnClickAsObservable
                  .Subscribe(_ => mainModel.Pass())
                  .AddTo(phaseRangedDisposable);
        
        passButton.SetActive(true);
    }

    void onGameEnd()
    {
        if (Permanent != null)
        {
            Permanent.playerNum = mainModel.NumberOfPlayers.Value;
            Permanent.players = mainModel.Players.Select(player => player.ToPlayerScore()).ToArray();
        }
        FadeManager.Instance.LoadScene("GameEnd", 1.0f);
    }

	// Use this for initialization
	void Start () {
        mainModel = new MainModel();
        Permanent = GameObject.Find("PermanentObj")?.GetComponent<PermanentObj>();

        ruleManager = mainModel._timesIsDelicious;

        mainModel.CurrentFoodCards.ObserveAdd().Subscribe(item =>
        {
            var foodCardModel = item.Value;
            // var foodCardVM = new FoodCardVM((FoodCard)item.Value);
            var foodCardView = foodCardFactory.CreateFoodCard();
            // foodCardView はGameObjectのDestroyとともに不正値になる。なんとかしないと
            Observable.NextFrame().Subscribe(_ =>
            {
                foodCardView.SetID(foodCardModel.ID);
            });

            AgedDisposable = foodCardModel.Aged
                                          .Where(aged => aged <= foodCardModel.MaxAged)
                                          .Subscribe(aged => foodCardView.UpdateAgedPont(aged));

            PriceDisposable = foodCardModel.Price
                                           .Subscribe(price => foodCardView.UpdateSellPont(price));

            RottenDisposable = foodCardModel.Rotten
                                            .Where(rotten => rotten)
                                            .Subscribe(rotten => {
                foodCardView.UpdateAgedPont(null);
                foodCardView.RunPoisonEffect();
            });

            foodCardView.OnClickAsObservable.Subscribe(_ =>
            {
                var meta = new CardViewModel.CardMeta();
                meta.Aged = foodCardModel.Aged.Value;
                meta.CanBet = foodCardModel.CanBet;
                meta.Description = foodCardModel.Description;
                meta.ID = foodCardModel.ID;
                meta.MaxAged = foodCardModel.MaxAged;
                meta.Name = foodCardModel.Name;
                meta.NamesWhoBet = foodCardModel.NamesWhoBet;
                meta.Price = foodCardModel.Price.Value;

                if( mainModel.CurrentStatus.Value == MainModel.Status.Betting)
                {
                    var disposable = cardDetailPanel.OnBetButtonClickAsObservable.First().Subscribe(__=> 
                    {
                        foodCardView.SetLogo(mainModel.CurrentPlayer.Value.Name);
                        mainModel.BetFood(foodCardModel);
                    });
                    cardDetailPanel.OnCloseAsObservable.First().Subscribe(_s =>
                    {
                        disposable.Dispose();
                    });
                    cardDetailPanel.OpenNiku(meta, 0, mainModel.CurrentPlayer.Value.Name);
                }
                else if(mainModel.CurrentStatus.Value == MainModel.Status.DecisionMaking)
                {
                    var disposable = cardDetailPanel.OnSellButtonClickAsObservable.Subscribe(__ =>
                    {
                        foodCardView.RemoveLogo(mainModel.CurrentPlayer.Value.Name);
                        mainModel.SellFood(foodCardModel);
                    });
                    cardDetailPanel.OnCloseAsObservable.First().Subscribe(_s =>
                    {
                        disposable.Dispose();
                    });
                    cardDetailPanel.OpenNiku(meta, 1, mainModel.CurrentPlayer.Value.Name);
                }
            });

            cardViewList.Add(foodCardView);
        });

        mainModel.CurrentFoodCards.ObserveRemove().Subscribe(item =>
        {
            var removedItem = item.Value;
            if (removedItem != null)
            {
                
                var removeView = cardViewList.FirstOrDefault(view => view.ID == removedItem.ID);
                if(removeView != null )
                {
                    removedItem.Reset();
                    foodCardFactory.RemoveFoodCard(removeView);
                }
            }
        });

        mainModel.CurrentEventCard
                           .Where(card => card != null)
                           .Subscribe(card=>
        {
            eventCardController.SetEventValues(
                card.ID,
                card.Name,
                card.Description,
                card.Weather[RuleManager.EventType.Temperature],
                card.Weather[RuleManager.EventType.Humid],
                card.Weather[RuleManager.EventType.Wind]);
        });

        mainModel.CurrentStatus
                 .Subscribe(status=>onStatusChanged(status))
                 .AddTo(this);

        mainModel.CurrentPlayer.Subscribe(player =>
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
                if (mainModel.CurrentStatus.Value == MainModel.Status.DecisionMaking)
                {
                    // 売るorパス決定ステータスに入ったとき、カレントプレイヤが肉をもってなかったら自動パス
                    if (cnt == 0)
                    {
                        mainModel.Pass();
                    }
                }
            });

            // gameDirector.CurrentPlayerName = player.Name;
            if (mainModel.CurrentStatus.Value == MainModel.Status.Betting)
            {
                popupMessageController.Popup(player.Name + "さんは肉を選んでください。");
            }
            if (mainModel.CurrentStatus.Value == MainModel.Status.DecisionMaking)
            {
                // 売るorパス決定ステータスに入ったとき、カレントプレイヤが肉をもってなかったら自動パス
                if (player.Bets.Count == 0)
                {
                    mainModel.Pass();
                }
            }
        });

        mainModel.TurnCount.Subscribe(cnt => {
            infoPanelController.UpdateTurn(cnt);
        });

        mainModel.RoundCount.Subscribe(cnt =>
        {
            infoPanelController.UpdateRound(cnt);
        });



        initPlayersUIWindowVM();

        mainModel.Start();
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
            mainModel.StartTimeIsDeliciousRound();
        });
    }

    private IEnumerator ControlTurnCoroutine(int dice)
    {
        yield return new WaitUntil(() =>
        {
            return mainModel.CurrentStatus.Value == MainModel.Status.Event; // イベントカードオープン待ちになったら
        });

        bool EventChecked = false;
        // イベントカードオープン
        mainModel.EventCardOpen();
        eventCardController.DrawEventCard().Subscribe( _ =>
        {
            cardDetailPanel.OnCloseAsObservable.Subscribe(___=>
            {
                gameDirector.DebugDiceClean();
                EventChecked = true;
            });
            cardDetailPanel.OpenEvent(mainModel.CurrentEventCard.Value.ID);
        });

        yield return new WaitUntil(() =>
        {
            return EventChecked && mainModel.CurrentStatus.Value == MainModel.Status.Aging; // 熟成待ち状態になったら
        });

        // 熟成
        mainModel.AdvanceTime(dice);

        yield return new WaitUntil(() =>
        {
            return EventChecked && mainModel.CurrentStatus.Value == MainModel.Status.NextTurn; // 次のターンへの移行待ち状態になったら
        });

        // ２秒待って
        yield return new WaitForSeconds(2.0f);
        mainModel.GoNextTurn(); // 次のターンへ移行
    }

    // Player Model <-> View 初期化
    private void onPlayer(Player player)
    {
        // UI追加
        var playerUI = playerWindowController.AddPlayer(player.Name, player.ID);

        player.Bets.ObserveRemove().Subscribe((bet) =>
        {
            var card = bet.Value;
            if (card.Rotten.Value)
            {
                // 腐ったことにより手放した
                playerUI.Sadden();
            }
        }).AddTo(playerUI);

        // 得点 <-> UI
        player.TotalEarned
              .Subscribe(earned => playerUI.UpdateTotalEarned(earned))
              .AddTo(playerUI);
    }

    private void initPlayersUIWindowVM()
    {
        mainModel.CurrentPlayer
                           .Where((arg) => arg != null)
                           .Subscribe(val =>
                           {
                                playerWindowController.SetCurrentPlayer(val.ID);
                           });
    }
}
