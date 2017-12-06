﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

// Player UI 全体のView
public class PlayersUIWindowController : MonoBehaviour {

    [SerializeField]
    private GameObject playerUIPrefab; // 将来的にはPrefabからInstantiateする

    private int numberOfPlayers;
    private PlayersUIWindowVM _playersWindowVM;
    private List<GameObject> _playerViewList; // 子Viewのリスト

    // Use this for initialization
    void Start () {

        _playerViewList = new List<GameObject>();
        numberOfPlayers = 0;

        //// 一度すべて非アクティブ
        for (int i = 0; i < 4; i++)
        {
            var name = "PlayerPrefab_" + i.ToString();
            var UI = transform.Find(name).gameObject;
            UI.SetActive(false);
        }

        _playersWindowVM = new PlayersUIWindowVM();
        _playersWindowVM.PlayerListVM.CollectionChanged += PlayerListVM_CollectionChanged;
    }

    private void PlayerListVM_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:

                foreach (var item in e.NewItems)
                {
                    // Instanciateの位置調整がうまくいかないのでひとまず静的配置からFind
                    var name = "PlayerPrefab_" + numberOfPlayers.ToString();
                    var UI = transform.Find(name).gameObject;
                    //    UI.transform.parent = transform;
                    //    var rectTrans = (RectTransform)UI.transform;
                    //    rectTrans.anchorMin = new Vector2(0f, 2/9f);
                    //    rectTrans.anchorMax = new Vector2(1f, 1f);
                    //    rectTrans.offsetMin = new Vector2(0.2f, 0.2f);
                    //    rectTrans.offsetMax = new Vector2(0.8f, 0.8f);
                    UI.SetActive(true);
                    UI.GetComponent<PlayerUIController>().setViewModel((PlayerVM)item);
                    UI.GetComponent<PlayerUIController>().ChangePosision(4-numberOfPlayers);
                    _playerViewList.Add(UI);
                    numberOfPlayers++;
                }
                Debug.Log("CurrentFoodCardsVM Add");
                break;
            case NotifyCollectionChangedAction.Move:
                Debug.Log("CurrentFoodCardsVM Move");
                break;
            case NotifyCollectionChangedAction.Remove:
                Debug.Log("CurrentFoodCardsVM Remove");
                break;
            case NotifyCollectionChangedAction.Replace:
                Debug.Log("CurrentFoodCardsVM Replace");
                break;
            case NotifyCollectionChangedAction.Reset:
                Debug.Log("CurrentFoodCardsVM Reset");
                break;
        }
    }
}