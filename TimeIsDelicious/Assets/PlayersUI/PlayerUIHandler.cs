using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIHandler : MonoBehaviour {

    [SerializeField]
    private GameObject playerUIPrefab;

    [SerializeField]
    private int numberOfPlayers;
    private List<GameObject> playerUIList;
    private int turn = 0;

    // Use this for initialization
    void Start () {
		
        if(numberOfPlayers<1)
        {
            Debug.Log("Error!Set at least 1 player!");
            return;
        }

        playerUIList = new List<GameObject>();
        for (int i=0; i< numberOfPlayers; i++)
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
            controller.ChangePosision(numberOfPlayers-i);
            playerUIList.Add(UI);
        }
        for(int i= numberOfPlayers; i < 4; i++)
        {
            var name = "PlayerPrefab_" + i.ToString();
            var UI = transform.Find(name).gameObject;
            UI.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Ratate()
    {
        turn += 1;

        for ( int i = 0; i < numberOfPlayers; i++)
        {
            playerUIList[i].GetComponent<PlayerUIController>().ChangePosision(((numberOfPlayers-i) + turn) % numberOfPlayers);
        }        
    }

    public void AddScore()
    {
        playerUIList[turn % numberOfPlayers].GetComponent<PlayerUIController>().SetScore(turn);
    }
}
