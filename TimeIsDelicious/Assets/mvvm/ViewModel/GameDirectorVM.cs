using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirectorVM : VMBase {

    private MainModel _singletonMainModel;

    public GameDirectorVM()
    {
        _singletonMainModel = MainModel.Instance;
        _singletonMainModel.StartTimeIsDelicious();
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

    public void SellFood()
    {
        _singletonMainModel.SellCurrentPlayersFood();
    }

    public void BetFood()
    {
        _singletonMainModel.BetFood();
    }
}
