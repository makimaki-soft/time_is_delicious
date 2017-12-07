﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

// UIからの通知はこのViewが受け取る(動的に表示される「はい」「いいえ」ボタンやシステムボタンなど)
public class GameDirector : MonoBehaviour
{
    private GameDirectorVM _mainVM; // ゲーム全体のView Model

    // Use this for initialization
    void Start () {
      
        _mainVM = new GameDirectorVM();
        _mainVM.PropertyChanged += _mainVM_PropertyChanged;
        _mainVM.StartRound(); // 実際は遷移アニメーション後にCallする

        _mainVM.BetFood(); // デバッグ用。
    }

    private void _mainVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var vm = (GameDirectorVM)sender;
        switch(e.PropertyName)
        {
            case "CurrentStatus":
                Debug.Log("Status Changed" + vm.CurrentStatus.ToString());
                break;
            case "CurrentPlayerName":
                if(vm.CurrentStatus == GameDirectorVM.Status.Betting)
                {
                    popupWindow.GetComponent<PopupMessaegController>().Popup(vm.CurrentPlayerName + "さんは肉を選んでください。");
                }
                break;
        }
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
        var dice = Dice.Value("");
        Debug.Log ("Stop Dice: " + Dice.Value (""));
		Dice.Clear ();
        dc.StopDice -= DebugStopHandler;
        _mainVM.AdvanceTime(dice);
    }
}
