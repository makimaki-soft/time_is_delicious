using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

// UIからの通知はこのViewが受け取る(動的に表示される「はい」「いいえ」ボタンやシステムボタンなど)
public class GameDirector : MonoBehaviour
{
    private GameDirectorVM _mainVM; // ゲーム全体のView Model

    private GameObject playerUIWindow;

    public int TurnCount { get; set; }
    public int RoundCount { get; set; }
    public MainModel.Status Status { get; set; }
    public string CurrentPlayerName { get; private set; }

    // Use this for initialization
    void Start()
    {

        TurnCount = 0;
        RoundCount = 0;

        
        _mainVM = new GameDirectorVM(this);

        // 監視対象GameObjectを取得
        playerUIWindow = GameObject.Find("PlayerUIPanel");

        // ゲームを開始する
        var pObj = GameObject.Find("PermanentObj")?.GetComponent<PermanentObj>();
        int? numOfPlayers = pObj?.playerNum;
        _mainVM.StartTimeIsDelicious( numOfPlayers.HasValue ? numOfPlayers.Value : 4);
    }


    public void GameRoundStart()
    {
        StartCoroutine(RoundStart());
    }

    public void SetCurrentPlayer(string Name)
    {
        CurrentPlayerName = Name;
    }

    public void RaisePopUp(string Name)
    {
        popupWindow.GetComponent<PopupMessaegController>().Popup(Name + "さんは肉を選んでください。");
    }

    private IEnumerator RoundStart()
    {
        // PlyerUIWindowが準備完了したら
        yield return new WaitUntil(() =>
        {
            return playerUIWindow.GetComponent<PlayersUIWindowController>().UIRready;
        });

        // メッセージを表示して、確認されたらStartRound
        popupWindow.GetComponent<PopupMessaegController>().Popup("ラウンド開始します", () =>
        {
            _mainVM.StartRound();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    // For Debug
    public GameObject popupWindow;
    private DiceController dc;
    public void OnClickForDebug()
    {
        dc = GetComponent<DiceController>();
        dc.StopDice = DebugStopHandler;
        dc.Roll("red");
    }
    public void OnClickForDebug2()
    {
        _mainVM.SellFood();
    }
    private void DebugStopHandler()
    {
        var dice = Dice.Value("");
        Debug.Log("Stop Dice: " + Dice.Value(""));
        // _mainVM.AdvanceTime(dice);
        _mainVM.NotifyDiceCasted(); // 判断待ちステータスに進める

        // 熟成待ち状態になったらダイスの出目で熟成
        StartCoroutine(ControlTurnCoroutine(dice));
    }

    private void DebugDiceClean()
    {
        Dice.Clear();
        dc.StopDice -= DebugStopHandler;
    }

    private IEnumerator ControlTurnCoroutine(int dice)
    {
        yield return new WaitUntil(() =>
        {
            return Status == MainModel.Status.Event; // イベントカードオープン待ちになったら
        });

        bool EventChecked = false;
        // イベントカードオープン
        _mainVM.EventCardOpen();
        GetComponent<EventCardController>().DrawEventCard(
            () =>
            {
                DebugDiceClean();
                EventChecked = true;
            });

        yield return new WaitUntil(() =>
        {
            return EventChecked && Status == MainModel.Status.Aging; // 熟成待ち状態になったら
        });

        // 熟成
        _mainVM.AdvanceTime(dice);

        yield return new WaitUntil(() =>
        {
            return EventChecked && Status == MainModel.Status.NextTurn; // 次のターンへの移行待ち状態になったら
        });

        // ２秒待って
        yield return new WaitForSeconds(2.0f);
        _mainVM.GoNextTurn(); // 次のターンへ移行
    }
}
