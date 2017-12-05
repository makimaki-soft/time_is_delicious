using RuleManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TempManager : MonoBehaviour
{

    public GameObject cardVMPrefab;
    private TimeIsDelicious timeIsDelicious; // Game Manager
    private PlayersUIWindowVM playersUIVM;
    private List<FoodCard> _foodCardList;

    // Use this for initialization
    void Start()
    {

        timeIsDelicious = new TimeIsDelicious();

        // Log
        foreach (var player in timeIsDelicious.Players)
        {
            Debug.Log(player.ID);
            player.PropertyChanged += (s, e) =>
            {
                Player sender = s as Player;
                if (e.PropertyName != "TotalEarned")
                {
                    return;
                }

                Debug.Log(sender.ID + "さんの収入:" + sender.TotalEarned);
            };
        }

        var cards = timeIsDelicious.StartRound();
        _foodCardList = cards;

        // debug
        int id = 0;
        foreach (var player in timeIsDelicious.Players)
        {
            player.Bet(cards[id++]);
        } 

        for (int i = 0; i < cards.Count; i++)
        {
            GameObject cardvm = (GameObject)Instantiate(
                cardVMPrefab,
                new Vector3(-100 + 30 * i, 11, 0),
                Quaternion.identity
            );
            cardvm.GetComponent<CardViewModel>().nikuNo = i + 1;
            cardvm.GetComponent<CardViewModel>().setModel(cards[i]);
        }

        playersUIVM = new PlayersUIWindowVM(timeIsDelicious.Players);
        var tmp = GameObject.Find("PlayerUIPanel").GetComponent<PlayersUIWindowController>();
        tmp.SetVM(playersUIVM);
    }

    // For Debug
    public void OnClickForDebug()
    {
        System.Random rdm = new System.Random();
        int dice = rdm.Next(1, 6);
        Debug.Log("サイコロの結果:" + dice);

        foreach (var player in timeIsDelicious.Players)
        {
            Debug.Log(player.ID + "さんは売りますか？(y/n):"); // 売らない
            //var res = Console.ReadLine();
            //if (res == "y")
            //{
            //   player.Sell(player.Bets[0]);
            //}
        }

        timeIsDelicious.AdvanceTime(dice);
    }
    public void OnClickForDebug2()
    {
        int i = 0;
        foreach (var player in timeIsDelicious.Players)
        {
            player.Sell(_foodCardList[i++]);
        }
    }
}
