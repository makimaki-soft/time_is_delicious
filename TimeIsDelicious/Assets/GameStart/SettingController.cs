using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingController : MonoBehaviour {

	private Color _selected;
	private Color _deselected;

	public GameObject player2;
	public GameObject player3;
	public GameObject player4;
	public GameObject playerNumText;

	private int _playerNum = 1;
	private Text _playerNumText;

	private PermanentObj _pObj;

	// Use this for initialization
	void Start () {
		_selected = new Color (1, 1, 1);
		_deselected = new Color (0.3f, 0.3f, 0.3f);

		_playerNumText = playerNumText.GetComponent<Text> ();

		_pObj = GameObject.Find ("PermanentObj").GetComponent<PermanentObj> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Select(GameObject obj) {

		// player1は必須
		Debug.Log("Name is " + obj.name);
		switch (obj.name)
		{
		case "player2":
			if (!selected (player3.GetComponent<Image> ().color) &&
			    !selected (player4.GetComponent<Image> ().color)) {
				SetColor (obj);
			}
			break;
		case "player3":
			if (selected(player2.GetComponent<Image> ().color) &&
				!selected (player4.GetComponent<Image> ().color)){
				SetColor (obj); 
			}
			break;
		case "player4":
			if (selected(player2.GetComponent<Image> ().color) &&
				selected(player3.GetComponent<Image> ().color)){
				SetColor (obj);
			}
			break;
		default:
			break;
		}

		_playerNumText.text = _playerNum.ToString ();
	}

	private void SetColor (GameObject obj) {
		Color cColor = obj.GetComponent<Image> ().color;
		if (selected (cColor)) {
			obj.GetComponent<Image> ().color = _deselected;
			_playerNum--;
		} else {
			obj.GetComponent<Image> ().color = _selected;
			_playerNum++;
		}
	}

	// 選択済みかどうか
	// 気持ち悪けどcolorで判定
	private bool selected(Color color) {
		if (color == _selected) {
			Debug.Log ("selected");
			return true;
		} else  {
			return false;
		}
	}

	public void GameStart() {

		// 	プレイヤー数を恒久オブジェクトに保存
		_pObj.playerNum = _playerNum;
        MainModel.Reset();
        Application.LoadLevel ("main");
	}
}
