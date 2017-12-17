using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardDetailPanelController : MonoBehaviour, IPointerClickHandler {

	public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);
	private float duration = 0.5f;

	private Image _cardImage;
	private Text _cardName;
	private Image _logo1;
	private Image _logo2;
	private GameObject _betButton;
	private GameObject _sellButton;

	public delegate void callBackClose();
	private callBackClose _callBackClose;

	public delegate void callBackBet();
	private callBackBet _callBackBet;

	public delegate void callBackSell();
	private callBackSell _callBackSell;

	// Use this for initialization
	void Start () {
		// 初期化
		gameObject.SetActive (false);

		GameObject basePanel = transform.Find ("BasePanel").gameObject;
		_cardImage = basePanel.transform.Find ("CardImage").GetComponent<Image> ();

		_logo1 = basePanel.transform.Find ("Logo1").GetComponent<Image> ();
		_logo2 = basePanel.transform.Find ("Logo2").GetComponent<Image> ();

		_betButton = basePanel.transform.Find ("BetButton").gameObject;
		_sellButton = basePanel.transform.Find ("SellButton").gameObject;
		_betButton.SetActive (false);
		_sellButton.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*
	 *  クリックハンドラ
	 */
	public void OnPointerClick(PointerEventData data) {
		Close ();
	}

	public void Open(callBackClose _funcClose = null,
		callBackBet _funcBet = null,
		callBackSell _funcSell = null) {

		// todo データをセットする

		_callBackClose = _funcClose;
		_callBackBet = _funcBet;
		_callBackSell = _funcSell;
		gameObject.SetActive (true);
	}

	public void Close() {
		gameObject.SetActive (false);

		_callBackClose?.Invoke();
		_callBackClose = null;
	}

	public void OnClickBet() {
		_callBackBet?.Invoke ();
		_callBackBet = null;
	}

	public void OnClickSell() {
		_callBackSell?.Invoke ();
		_callBackSell = null;
	}
}
