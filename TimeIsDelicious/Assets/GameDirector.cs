using System.Collections;
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
        _mainVM.StartRound(); // 実際は遷移アニメーション後にCallする

        _mainVM.BetFood(); // デバッグ用。
    }

    // Update is called once per frame
    void Update () {
		
	}

    // For Debug
    public void OnClickForDebug()
    {
        System.Random rdm = new System.Random();
        int dice = rdm.Next(1, 6);
        Debug.Log("サイコロの結果:" + dice);

        _mainVM.AdvanceTime(dice);
    }
    public void OnClickForDebug2()
    {

        _mainVM.SellFood();
    }
}
