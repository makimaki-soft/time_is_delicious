using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UniRx;

public class GameDirectorVM : VMBase {

    private MainModel _singletonMainModel;
    public GameDirector gameDirector { private get; set; }

    public GameDirectorVM(GameDirector gd)
    {
        gameDirector = gd;
        _singletonMainModel = MainModel.Instance;

        _singletonMainModel.CurrentStatus.Subscribe(status=>
        {
            gameDirector.Status = status;
            if (status == MainModel.Status.WaitForRoundStart)
            {
                gameDirector.GameRoundStart();
            }
        });

        _singletonMainModel.CurrentPlayer.Subscribe(player=>
        {
            if(player == null)
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

            gameDirector.SetCurrentPlayer(player.Name);
            if (_singletonMainModel.CurrentStatus.Value == MainModel.Status.Betting)
            {
                gameDirector.RaisePopUp(player.Name);
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

        _singletonMainModel.TurnCount.Subscribe(cnt=>{
            gameDirector.TurnCount = cnt;
        });

        _singletonMainModel.RoundCount.Subscribe(cnt=>
        {
            gameDirector.RoundCount = cnt;
        });
    }

    private IDisposable currentPlayerBetsDisposable;

    public void StartTimeIsDelicious(int numOfPlayers)
    {
        _singletonMainModel.StartTimeIsDelicious(numOfPlayers);
    }
    public void StartRound()
    {
        _singletonMainModel.StartTimeIsDeliciousRound();
    }

    public void AdvanceTime(int i)
    {
        // todo エラーチェック
        _singletonMainModel.AdvanceTime(i);
    }

    public void EventCardOpen()
    {
        _singletonMainModel.EventCardOpen();
    }

    public void GoNextTurn()
    {
        _singletonMainModel.GoNextTurn();
    }

    public void SellFood()
    {
        // _singletonMainModel.SellCurrentPlayersFood();
    }

    public void BetFood()
    {
        // _singletonMainModel.BetFood(null);
    }

	public void NotifyDiceCasted()
	{
		_singletonMainModel.NotifyDiceCasted ();
	}
}
