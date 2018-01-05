using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UniRx;

// UIからの通知はこのViewが受け取る(動的に表示される「はい」「いいえ」ボタンやシステムボタンなど)
public class GameDirector : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
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
        onClickSubject.OnNext(Unit.Default);
    }

    private void DebugStopHandler()
    {
        var dice = Dice.Value("");
        Debug.Log("Stop Dice: " + Dice.Value(""));
        onDice.OnNext(dice);
    }

    public IObservable<int> DiceRoll()
    {
        dc = GetComponent<DiceController>();
        dc.StopDice = DebugStopHandler;
        dc.Roll("red");

        return onDice;
    }

    public IObservable<Unit> OnDiceButtonClickAsObservable
    {
        get { return onClickSubject; }
    }
    private Subject<UniRx.Unit> onClickSubject = new Subject<Unit>();
    private Subject<int> onDice = new Subject<int>();

    public void DebugDiceClean()
    {
        Dice.Clear();
        dc.StopDice -= DebugStopHandler;
    }
}
