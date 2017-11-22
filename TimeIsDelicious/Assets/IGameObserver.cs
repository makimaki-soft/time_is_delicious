using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

interface IGameObserver : INotifyPropertyChanged
{
    //! 人数を指定してゲームを開始する
    void StartGameWith(int numberOfPlayers);

    //! ターンのプレイヤーを取得する
    int GetCurrentPlayer();

    //! そのカードに賭けられるか
    bool CanBet(int player, int card);

    //! カードに賭ける
    void Bet(int player, int card);

    //! 次のイベントカードをオープンする
    void OpenNextEventCard();

    //! さいころを振る
    void CastDice();
}