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

    public static EventCardController.EventValues ToEventMeta(this EventCard eventCard)
    {
        var meta = new EventCardController.EventValues();
        meta.ID = eventCard.ID;
        meta.Name = eventCard.Name;
        meta.Description = eventCard.Description;
        meta.Temperature = eventCard.Weather[RuleManager.EventType.Temperature];
        meta.Humidity = eventCard.Weather[RuleManager.EventType.Humid];
        meta.Wind = eventCard.Weather[RuleManager.EventType.Wind];
        return meta;
    }

}

public class MainPresenter : MonoBehaviour {
    
    [SerializeField]
    FoodCardGenerator foodCardGenerator;

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
    private TimeIsDelicious ruleManager;
    private PhaseManager phaseManager;

    public PermanentObj Permanent { get; private set; }

    List<FoodCard> foodCardModelList = new List<FoodCard>();

    void onStatusChanged(PhaseManager.Status status)
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
            case PhaseManager.Status.NotStarted:
                break;
            case PhaseManager.Status.Init:
                onGameStart();
                break;
            case PhaseManager.Status.WaitForRoundStart:  // ラウンド開始待ち
                onRoundStart();
                break;
            case PhaseManager.Status.Betting:
                onBetting();
                break;// 賭け中
            case PhaseManager.Status.CastDice:           // サイコロ待ち
                onCastDice();
                break;
            case PhaseManager.Status.DecisionMaking:     // 売る・熟成判断待ち
                onDecisionMaking();
                break;
            case PhaseManager.Status.Event:              // イベントカードオープン待ち
                onEvent();
                break;
            case PhaseManager.Status.Aging:              // 熟成待ち
                onAging();
                break;
            case PhaseManager.Status.NextTurn:           // 次のターンへの移行待ち
                onNextTurn();
                break;
            case PhaseManager.Status.GameEnd:             // ゲーム終了
                onGameEnd();
                break;
        }
    }

    void onGameStart()
    {
        int numOfPlayers = Permanent ? Permanent.playerNum : 4;

        // Player生成
        ruleManager.CreatePlayers(numOfPlayers)
                   .Select(player => onPlayer(player))
                   .ToArray(); // Queryを強制評価するためToArrayを呼ぶ。
        
        phaseManager.TurnCount
                    .Subscribe(cnt => infoPanelController.UpdateTurn(cnt))
                    .AddTo(infoPanelController);

        phaseManager.RoundCount
                    .Subscribe(cnt => infoPanelController.UpdateRound(cnt))
                    .AddTo(infoPanelController);

        playerWindowController.OnGUIAsObservable
                              .Skip(numOfPlayers - 1)
                              .First()
                              .Subscribe(_ => phaseManager.StartTimeIsDelicious());
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
                foodCardGenerator.RemoveFoodCard(removeView);
            }
        }
        foodCardModelList.Clear();

        // 今ラウンドの初期化処理
        // メッセージを表示して、確認されたらStartRound
        popupMessageController.Popup("ラウンド開始します")
                              .First()
                              .Do(_=>initRoundFoodCard())
                              .Subscribe(_ => phaseManager.NotifyRoundInitialized());
    }

    void onBetting()
    {
        phaseManager.CurrentPlayer
                    .Where((arg) => arg != null)
                    .Subscribe(player => 
                    {
                        playerWindowController.SetCurrentPlayer(player.ID);
                        popupMessageController.Popup(player.Name + "さんは肉を選んでください。");
                    })
                    .AddTo(phaseRangedDisposable);

        foreach( var foodCardModel in foodCardModelList )
        {
            var foodCardView = cardViewList[foodCardModel.GUID];

            foodCardView.OnClickAsObservable
                        .Do(_ => cardDetailPanel.OpenNiku(foodCardModel.ToCardMeta(), 0, phaseManager.CurrentPlayer.Value.Name))
                        .SelectMany(_ => Observable.Amb(
                            cardDetailPanel.OnBetButtonClickAsObservable.First().Select(__ => true),
                            cardDetailPanel.OnCloseAsObservable.First().Select(__ => false)
                         ))
                        .Where(isBet => isBet)
                        .Subscribe(_ =>
                        {
                            foodCardView.SetLogo(phaseManager.CurrentPlayer.Value.Name);
                            phaseManager.CurrentPlayer.Value.Bet(foodCardModel);
                        })
                        .AddTo(phaseRangedDisposable);
        }
    }

    int dice;
    void onCastDice()
    {
        diceController.IsActive = true;
               
        gameDirector.OnDiceButtonClickAsObservable
                    .SelectMany(_=>gameDirector.DiceRoll())
                    .First()
                    .Subscribe(dice=>
                    {
                        this.dice = dice;
                        phaseManager.NotifyDiceCasted(); // 判断待ちステータスに進める
                    })
                    .AddTo(phaseRangedDisposable);
    }

    void onDecisionMaking()
    {
        passButton.OnClickAsObservable
                  .Subscribe(_ => phaseManager.Pass())
                  .AddTo(phaseRangedDisposable);
        
        passButton.SetActive(true);

        phaseManager.CurrentPlayer
                    .Where((arg) => arg != null)
                    .Subscribe(player =>
                    {
                        playerWindowController.SetCurrentPlayer(player.ID);
                        playerRangedDisposable.Clear();

                        // 売るorパス決定ステータスに入ったとき、カレントプレイヤが肉をもってなかったら自動パス
                        player.Bets.ObserveCountChanged(true)
                              .Where(cnt=>cnt == 0)
                              .AsUnitObservable()
                              .Subscribe(_=>phaseManager.Pass())
                              .AddTo(playerRangedDisposable);
                        
                    })
                    .AddTo(phaseRangedDisposable);

        foreach (var foodCardModel in foodCardModelList)
        {
            var foodCardView = cardViewList[foodCardModel.GUID];

            foodCardView.OnClickAsObservable
                        .Do(_ => cardDetailPanel.OpenNiku(foodCardModel.ToCardMeta(), 1, phaseManager.CurrentPlayer.Value.Name))
                        .SelectMany(_ => Observable.Amb(
                            cardDetailPanel.OnSellButtonClickAsObservable.Select(__ => true),
                            cardDetailPanel.OnCloseAsObservable.Select(__ => false)
                           ))
                        .Where(isSell => isSell)
                        .Subscribe(_ =>
                        {
                            foodCardView.RemoveLogo(phaseManager.CurrentPlayer.Value.Name);
                            phaseManager.CurrentPlayer.Value.Sell(foodCardModel);
                        })
                        .AddTo(phaseRangedDisposable);
        }
    }

    void onEvent()
    {
        // イベントカードオープン
        var card = ruleManager.OpenEventCard();

        eventCardController.DrawEventCard(card.ToEventMeta())
                           .First()
                           .Do(_=>cardDetailPanel.OpenEvent(card.ID))
                           .Do(_=>phaseManager.NotifyEventCardChecked())
                           .SelectMany(_=> cardDetailPanel.OnCloseAsObservable)
                           .First()
                           .Subscribe(_ =>
                           {
                                gameDirector.DebugDiceClean();
                                ruleManager.AdvanceTime(this.dice, card);
                                phaseManager.NotifyAged();
                           });
    }

    void onAging()
    {
        // 熟成処理はEvent Phaseに統合
    }

    void onNextTurn()
    {
        // 次ターンへの移行は自動で行う
    }

    void onGameEnd()
    {
        if (Permanent != null)
        {
            Permanent.playerNum = ruleManager.NumberOfPlayers;
            Permanent.players = ruleManager.Players.Select(player => player.ToPlayerScore()).ToArray();
        }
        FadeManager.Instance.LoadScene("GameEnd", 1.0f);
    }

	// Use this for initialization
	void Start () {

        ruleManager = new TimeIsDelicious();
        phaseManager = new PhaseManager(ruleManager);

        Permanent = GameObject.Find("PermanentObj")?.GetComponent<PermanentObj>();

        phaseManager.CurrentStatus
                    .Subscribe(status=>onStatusChanged(status))
                    .AddTo(this);
        
        phaseManager.Start();
    }

    // Player Model <-> View 初期化
    private PlayerUIController onPlayer(Player player)
    {
        MakiMaki.Logger.Info("onPlayer");
        // UI追加
        var playerUI = playerWindowController.AddPlayer(player.Name, player.ID);

        // 腐ったことにより手放したら泣く
        player.Bets.ObserveRemove()
              .Where(removed => removed.Value.Rotten.Value)
              .AsUnitObservable()
              .Subscribe(_ => playerUI.Sadden())
              .AddTo(playerUI);
        
        // 得点 <-> UI
        player.TotalEarned
              .Subscribe(earned => playerUI.UpdateTotalEarned(earned))
              .AddTo(playerUI);

        return playerUI;
    }

    void initRoundFoodCard()
    {
        foodCardModelList = ruleManager.StartRound();
        cardViewList = foodCardModelList.ToDictionary(foodCard => foodCard.GUID, 
                                                      foodCard => createFoodCardView(foodCard));
    }

    private CardViewModel createFoodCardView(FoodCard foodCardModel)
    {
        var foodCardView = foodCardGenerator.CreateFoodCard();

        // Createした瞬間はStart()が呼ばれていない
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
                     .Subscribe(rotten => 
                     {
                        foodCardView.UpdateAgedPont(null);
                        foodCardView.RunPoisonEffect();
                     })
                     .AddTo(foodCardView);

        return foodCardView;
    }

    private void OnDestroy()
    {
        phaseRangedDisposable.Clear();
        playerRangedDisposable.Clear();
        roundRangedDisposable.Clear();
    }
}
