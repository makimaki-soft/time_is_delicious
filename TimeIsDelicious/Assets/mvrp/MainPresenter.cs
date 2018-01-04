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

    private List<CardViewModel> cardViewList = new List<CardViewModel>();

    private MainModel _singletonMainModel;
    public PermanentObj Permanent { get; private set; }


    IDisposable AgedDisposable;
    IDisposable PriceDisposable;
    IDisposable RottenDisposable;

	// Use this for initialization
	void Start () {
        _singletonMainModel = MainModel.Instance;
        Permanent = GameObject.Find("PermanentObj")?.GetComponent<PermanentObj>();

        _singletonMainModel.CurrentFoodCards.ObserveAdd().Subscribe(item =>
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

                if( _singletonMainModel.CurrentStatus.Value == MainModel.Status.Betting)
                {
                    var disposable = cardDetailPanel.OnBetButtonClickAsObservable.First().Subscribe(__=> 
                    {
                        foodCardView.SetLogo(_singletonMainModel.CurrentPlayer.Value.Name);
                        _singletonMainModel.BetFood(foodCardModel);
                    });
                    cardDetailPanel.OnCloseAsObservable.First().Subscribe(_s =>
                    {
                        disposable.Dispose();
                    });
                    cardDetailPanel.OpenNiku(meta, 0, _singletonMainModel.CurrentPlayer.Value.Name);
                }
                else if(_singletonMainModel.CurrentStatus.Value == MainModel.Status.DecisionMaking)
                {
                    var disposable = cardDetailPanel.OnSellButtonClickAsObservable.Subscribe(__ =>
                    {
                        foodCardView.RemoveLogo(_singletonMainModel.CurrentPlayer.Value.Name);
                        _singletonMainModel.SellFood(foodCardModel);
                    });
                    cardDetailPanel.OnCloseAsObservable.First().Subscribe(_s =>
                    {
                        disposable.Dispose();
                    });
                    cardDetailPanel.OpenNiku(meta, 1, _singletonMainModel.CurrentPlayer.Value.Name);
                }
            });

            cardViewList.Add(foodCardView);
        });

        _singletonMainModel.CurrentFoodCards.ObserveRemove().Subscribe(item =>
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

        _singletonMainModel.CurrentEventCard
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

            // gameDirector.CurrentPlayerName = player.Name;
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
            infoPanelController.UpdateTurn(cnt);
        });

        _singletonMainModel.RoundCount.Subscribe(cnt =>
        {
            infoPanelController.UpdateRound(cnt);
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
        eventCardController.DrawEventCard().Subscribe( _ =>
        {
            cardDetailPanel.OnCloseAsObservable.Subscribe(___=>
            {
                gameDirector.DebugDiceClean();
                EventChecked = true;
            });
            cardDetailPanel.OpenEvent(_singletonMainModel.CurrentEventCard.Value.ID);
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

    IDisposable passButtonDisposable;

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

            if(val == MainModel.Status.DecisionMaking)
            {
                passButtonDisposable = passButton.OnClickAsObservable.Subscribe(_ => MainModel.Instance.Pass());
                passButton.SetActive(true);
            }
            else
            {
                passButtonDisposable?.Dispose();
                passButton.SetActive(false);
            }
        });

        // playerModel と UIの結合
        _singletonMainModel.Players.ObserveAdd().Subscribe(item =>
        {
            var playerModel = item.Value;
            var playerUI = playerWindowController.AddPlayer(playerModel.Name, playerModel.ID);

            playerModel.Bets.ObserveRemove().Subscribe((bet) =>
            {
                var card = bet.Value;
                if (card.Rotten.Value)
                {
                    // 腐ったことにより手放した
                    playerUI.Sadden();
                }
            });

            playerModel.TotalEarned.Subscribe(earned => playerUI.UpdateTotalEarned(earned));
        });
    }
}
