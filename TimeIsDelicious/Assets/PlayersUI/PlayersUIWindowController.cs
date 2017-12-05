using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersUIWindowController : MonoBehaviour {

    [SerializeField]
    private GameObject playerUIPrefab;

    [SerializeField]
    private int numberOfPlayers;
    private Dictionary<Guid,PlayerUIController> playerUIList;
    private int turn = 0;

    // Use this for initialization
    void Start () {

        playerUIList = new Dictionary<Guid, PlayerUIController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private PlayersUIWindowVM _vm;
    public void SetVM(PlayersUIWindowVM vm)
    {
        _vm = vm;

        numberOfPlayers = _vm.Score.Count;
        Guid[] guidList = new Guid[numberOfPlayers];
        _vm.Score.Keys.CopyTo(guidList, 0);

        for (int i = 0; i < numberOfPlayers; i++)
        {
            var name = "PlayerPrefab_" + i.ToString();
            var UI = transform.Find(name).gameObject;
            //    UI.transform.parent = transform;
            //    var rectTrans = (RectTransform)UI.transform;
            //    rectTrans.anchorMin = new Vector2(0f, 2/9f);
            //    rectTrans.anchorMax = new Vector2(1f, 1f);
            //    rectTrans.offsetMin = new Vector2(0.2f, 0.2f);
            //    rectTrans.offsetMax = new Vector2(0.8f, 0.8f);
            var controller = UI.GetComponent<PlayerUIController>();
            controller.ChangePosision(numberOfPlayers - i);
            playerUIList[guidList[i]] = controller;
        }
        for (int i = numberOfPlayers; i < 4; i++)
        {
            var name = "PlayerPrefab_" + i.ToString();
            var UI = transform.Find(name).gameObject;
            UI.SetActive(false);
        }

        _vm.PropertyChanged += _vm_PropertyChanged;
    }

    private void _vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var vm = sender as PlayersUIWindowVM;
        if(e.PropertyName == "Score")
        {
            foreach (KeyValuePair<Guid, int> pair in vm.Score)
            {
                playerUIList[pair.Key].SetScore(pair.Value);
            }
        }
    }

    public void Ratate()
    {
        turn += 1;

        //for ( int i = 0; i < numberOfPlayers; i++)
        //{
        //    playerUIList[i].ChangePosision(((numberOfPlayers-i) + turn) % numberOfPlayers);
        //}        
    }

    public void AddScore()
    {
        // playerUIList[turn % numberOfPlayers].SetScore(turn);
    }
}
