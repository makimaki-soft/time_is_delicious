using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameEndController : MonoBehaviour {

	public GameObject Background;
	public Image winPlayerImage;
	public Text winMessage;

	public Image thumbnails0;
	public Image thumbnails1;
	public Image thumbnails2;
	public Image thumbnails3;

	public Text score0;
	public Text score1;
	public Text score2;
	public Text score3;

	private PermanentObj _pObj;

	// Use this for initialization
	void Start () {
		_pObj = GameObject.Find ("PermanentObj")?.GetComponent<PermanentObj> ();
		Image[] thumbnails = { thumbnails0, thumbnails1, thumbnails2, thumbnails3 };
		Text[] scores = { score0, score1, score2, score3 };

		Debug.Log ("thumbnails " + thumbnails.Length);
		Debug.Log ("scores " + scores.Length);
		Debug.Log (_pObj.name);

		PlayerVM[] players = _pObj.players;
		Array.Sort (players, (a, b) => b.TotalEarned - a.TotalEarned);

		int i = 0;
		foreach(var player in players) {
			Debug.Log ("ID:" + player.ID.ToString () + ", Point:" + player.TotalEarned.ToString ());
			scores [i].GetComponent<Text> ().text = player.TotalEarned.ToString () + "G";
			thumbnails [i].GetComponent<Image> ().sprite =
				Resources.Load<Sprite> ("end/chara" + player.ID.ToString ());
			i++;
		}
		winMessage.text = players[0].Name + " WIN!";
		winPlayerImage.sprite =
			Resources.Load<Sprite> ("chara_all" + players [0].ID.ToString ());
		Background.GetComponent<Image> ().sprite =
			Resources.Load<Sprite> ("end/win" + players [0].ID.ToString ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ReturnToStart () {
		Debug.Log ("return to start");
        MainModel.Reset();
        Application.LoadLevel ("Title");
	}
}
