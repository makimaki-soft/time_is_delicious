using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using RuleManager;
using System.Linq;
using System;

static class ModelViewExtension
{
    public static CardViewModel.CardMeta ToCardMeta(this FoodCard foodCardModel)
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
        return meta;
    }
}

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

    private Dictionary<Guid,CardViewModel> cardViewList = new Dictionary<Guid, CardViewModel>();

    // Disposable Control
    private CompositeDisposable phaseRangedDisposable = new CompositeDisposable();
    private CompositeDisposable playerRangedDisposable = new CompositeDisposable();
    private CompositeDisposable roundRangedDisposable = new CompositeDisposable();

    // Model
    private MainModel mainModel;
    public PermanentObj Permanent { get; private set; }

    private TimeIsDelicious ruleManager;

    List<FoodCard> foodCardModelList = new List<FoodCard>();

    void onStatusChanged(MainModel.Status status)
    {
        phaseRangedDisposable.Clear();

        // Common
        statusPanel.gdStatus = status;
        diceController.IsActive = false;
        passButton.SetActive(false);

        MakiMaki.Logger.Info("OnPhase : " + status.ToString());

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
                onBetting();
                break;// 賭け中
            case MainModel.Status.CastDice:           // サイコロ待ち
                onCastDice();
                break;
            case MainModel.Status.DecisionMaking:     // 売る・熟成判断待ち
                onDecisionMaking();
                break;
            case MainModel.Status.Event:              // イベントカードオープン待ち
                onEvent();
                break;
            case MainModel.Status.Aging:              // 熟成待ち
                onAging();
                break;
            case MainModel.Status.NextTurn:           // 次のターンへの移行待ち
                onNextTurn();
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

        mainModel.TurnCount
                 .Subscribe(cnt => infoPanelController.UpdateTurn(cnt))
                 .AddTo(infoPanelController);

        mainModel.RoundCount
                 .Subscribe(cnt => infoPanelController.UpdateRound(cnt))
                 .AddTo(infoPanelController);

        // debug //
        foreach (var player in ruleManager.Players)
        {
            mainModel.Players.Add(player);
        }

        mainModel.StartTimeIsDelicious(nop);
    }

    void onRoundStart()
    {
        // 前ラウンドの破棄処理
        foreach(var foodCard in foodCardModelList)
        {
            var removeView = cardViewList.FirstOrDefault(view => view.Value.ID == foodCard.ID).Value;
            if (removeView != null)
            {
                foodCard.Reset();
                foodCardFactory.RemoveFoodCard(removeView);
            }
        }
        foodCardModelList.Clear();

        // 今ラウンドの初期化処理
        playerWindowController.OnGUIAsObservable
                              .First()
                              .Subscribe(_ =>
        {
            // メッセージを表示して、確認されたらStartRound
            popupMessageController.Popup("ラウンド開始します", () =>
            {
                mainModel.CurrentFoodCards.Clear();

                foreach (var foodCard in ruleManager.StartRound())
                {
                    onFoodCard(foodCard);
                    foodCardModelList.Add(foodCard);

                    // Debug
                    mainModel.CurrentFoodCards.Add(foodCard);
                    //
                }

                mainModel.StartTimeIsDeliciousRound();
            });
        }).AddTo(phaseRangedDisposable);
    }

    void onBetting()
    {
        mainModel.CurrentPlayer
                 .Where((arg) => arg != null)
                 .Subscribe(player => 
        {
            playerWindowController.SetCurrentPlayer(player.ID);
            popupMessageController.Popup(player.Name + "さんは肉を選んでください。");
        }).AddTo(phaseRangedDisposable);

        foreach( var foodCardModel in foodCardModelList )
        {
            var foodCardView = cardViewList[foodCardModel.GUID];

            foodCardView.OnClickAsObservable.Subscribe(_ =>
            {
                var disposable = cardDetailPanel.OnBetButtonClickAsObservable.First().Subscribe(__ =>
                {
                    foodCardView.SetLogo(mainModel.CurrentPlayer.Value.Name);
                    mainModel.BetFood(foodCardModel);
                });
                cardDetailPanel.OnCloseAsObservable.First().Subscribe(_s =>
                {
                    disposable.Dispose();
                });
                cardDetailPanel.OpenNiku(foodCardModel.ToCardMeta(), 0, mainModel.CurrentPlayer.Value.Name);

            }).AddTo(phaseRangedDisposable);
        }
    }

    int dice;
    void onCastDice()
    {
        diceController.IsActive = true;
                      
        gameDirector.OnDiceButtonClickAsObservable.Subscribe(_ =>
        {
            gameDirector.DiceRoll().First().Subscribe(dice =>
            {
                this.dice = dice;
                mainModel.NotifyDiceCasted(); // 判断待ちステータスに進める
            });
        }).AddTo(phaseRangedDisposable);
    }

    void onDecisionMaking()
    {
        passButton.OnClickAsObservable
                  .Subscribe(_ => mainModel.Pass())
                  .AddTo(phaseRangedDisposable);
        
        passButton.SetActive(true);

        mainModel.CurrentPlayer
                 .Where((arg) => arg != null)
                 .Subscribe(player =>
        {
            playerWindowController.SetCurrentPlayer(player.ID);
            playerRangedDisposable.Clear();
            player.Bets.ObserveCountChanged(true).Subscribe(cnt =>
            {
                // 売るorパス決定ステータスに入ったとき、カレントプレイヤが肉をもってなかったら自動パス
                if (cnt == 0)
                {
                    mainModel.Pass();
                }
            }).AddTo(playerRangedDisposable);
        }).AddTo(phaseRangedDisposable);

        foreach (var foodCardModel in foodCardModelList)
        {
            var foodCardView = cardViewList[foodCardModel.GUID];
            foodCardView.OnClickAsObservable.Subscribe(_ =>
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
                cardDetailPanel.OpenNiku(foodCardModel.ToCardMeta(), 1, mainModel.CurrentPlayer.Value.Name);
            }).AddTo(phaseRangedDisposable);
        }
    }

    void onEvent()
    {
        EventChecked = false;
        // イベントカードオープン
        mainModel.EventCardOpen();
        var card = mainModel.CurrentEventCard.Value;
        eventCardController.SetEventValues(
            card.ID,
            card.Name,
            card.Description,
            card.Weather[RuleManager.EventType.Temperature],
            card.Weather[RuleManager.EventType.Humid],
            card.Weather[RuleManager.EventType.Wind]
        );

        eventCardController.DrawEventCard().Subscribe(_ =>
        {
            cardDetailPanel.OnCloseAsObservable.Subscribe(___ =>
            {
                gameDirector.DebugDiceClean();
                EventChecked = true;
            });
            cardDetailPanel.OpenEvent(mainModel.CurrentEventCard.Value.ID);
        });
    }

    bool EventChecked;
    private IEnumerator ControlTurnCoroutine(int dicenum)
    {
        yield return new WaitUntil(()=> EventChecked);
        // 熟成
        mainModel.AdvanceTime(dicenum);
    }

    void onAging()
    {
        // 熟成待ち状態になったらダイスの出目で熟成
        StartCoroutine(ControlTurnCoroutine(this.dice));
    }

    void onNextTurn()
    {
        // ２秒後に次のターンへ移行
        Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe(_ =>
        {
            mainModel.GoNextTurn();
        }).AddTo(this);
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

        mainModel.CurrentStatus
                 .Subscribe(status=>onStatusChanged(status))
                 .AddTo(this);
        
        mainModel.Start();
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

    private void onFoodCard(FoodCard foodCardModel)
    {
        var foodCardView = foodCardFactory.CreateFoodCard();

        Observable.NextFrame().Subscribe(_ =>
        {
            foodCardView.SetID(foodCardModel.ID);
        });

        foodCardModel.Aged
                     .Where(aged => aged <= foodCardModel.MaxAged)
                     .Subscribe(aged => foodCardView.UpdateAgedPont(aged))
                     .AddTo(foodCardView);

        foodCardModel.Price
                     .Subscribe(price => foodCardView.UpdateSellPont(price))
                     .AddTo(foodCardView);

        foodCardModel.Rotten
                     .Where(rotten => rotten)
                     .Subscribe(rotten => {
            foodCardView.UpdateAgedPont(null);
            foodCardView.RunPoisonEffect();
        }).AddTo(foodCardView);

        cardViewList[foodCardModel.GUID] = foodCardView;
    }

    private void OnDestroy()
    {
        phaseRangedDisposable.Clear();
        playerRangedDisposable.Clear();
        roundRangedDisposable.Clear();
    }
}
