using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

// UIからの通知はこのViewが受け取る(動的に表示される「はい」「いいえ」ボタンやシステムボタンなど)
public class GameDirector : MonoBehaviour
{
    private GameDirectorVM _mainVM; // ゲーム全体のView Model

    private GameObject playerUIWindow;

    // Use this for initialization
    void Start () {
      
        _mainVM = new GameDirectorVM();
        _mainVM.PropertyChanged += _mainVM_PropertyChanged;
      
        // 監視対象GameObjectを取得
        playerUIWindow = GameObject.Find("PlayerUIPanel");


        // ゲームを開始する
        _mainVM.StartTimeIsDelicious();
    }

    private void _mainVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var vm = (GameDirectorVM)sender;
        switch(e.PropertyName)
        {
            case "CurrentStatus":
                if (vm.CurrentStatus == GameDirectorVM.Status.WaitForRoundStart)
                {
                    // ラウンド開始中のアニメーションをまってからStartRound
                    Debug.Log("StartCoroutine(RoundStart())");
                    StartCoroutine(RoundStart());
                }
                break;
            case "CurrentPlayerName":
                if(vm.CurrentStatus == GameDirectorVM.Status.Betting)
                {
                    popupWindow.GetComponent<PopupMessaegController>().Popup(vm.CurrentPlayerName + "さんは肉を選んでください。");
                    _mainVM.BetFood(); // For Debug
                }
                break;
        }
    }

    private IEnumerator RoundStart()
    {
        // PlyerUIWindowが準備完了したら
        yield return new WaitUntil(() => {
            return playerUIWindow.GetComponent<PlayersUIWindowController>().UIRready;
        });

        // メッセージを表示して、確認されたらStartRound
        popupWindow.GetComponent<PopupMessaegController>().Popup("ラウンド開始します", () =>
        {
            _mainVM.StartRound();
        });
    }

    // Update is called once per frame
    void Update () {
		
	}

    // For Debug
	public GameObject popupWindow;
	private DiceController dc;
    public void OnClickForDebug()
    {
		dc = GetComponent<DiceController> ();
		dc.StopDice = DebugStopHandler;
		dc.Roll ("red");
    }
    public void OnClickForDebug2()
    {
        _mainVM.SellFood();
    }
	private void DebugStopHandler ()
	{
		GetComponent<EventCardController> ().DrawEventCard (
			() => {
				DebugDiceClean();
			});

        var dice = Dice.Value("");
        Debug.Log ("Stop Dice: " + Dice.Value (""));
        _mainVM.AdvanceTime(dice);
    }

	private void DebugDiceClean () {
		Dice.Clear ();
		dc.StopDice -= DebugStopHandler;
	}
}
