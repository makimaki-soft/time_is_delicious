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
	private Text _cardDes;
	private Text _agingPointText;
	private Text _sellPointText;
	private GameObject _logo1;
	private GameObject _logo2;
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

		GameObject descriptionPanel = basePanel.transform.Find ("DescriptionPanel").gameObject;
		_cardName = descriptionPanel.transform.Find ("CardNameText").GetComponent<Text> ();
		_cardDes = descriptionPanel.transform.Find ("CardDescriptionText").GetComponent<Text> ();

		_agingPointText = basePanel.transform.Find ("AgingPointText").GetComponent<Text> ();
		_sellPointText = basePanel.transform.Find ("SellPointText").GetComponent<Text> ();

		_logo1 = basePanel.transform.Find ("Logo1").gameObject;
		_logo2 = basePanel.transform.Find ("Logo2").gameObject;
		_logo1.SetActive (false);
		_logo2.SetActive (false);

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

	public void OpenNiku(FoodCardVM _food,
		callBackClose _funcClose = null,
		callBackBet _funcBet = null,
		callBackSell _funcSell = null) {

		string nikuImgName = "Niku/card" +  _food.ID.ToString();
		Sprite image = Resources.Load<Sprite> (nikuImgName);
		_cardImage.sprite = image;

		_cardName.text = _food.Name;
		_cardDes.text = _food.Description;
		_agingPointText.text = _food.Aged.ToString () + "/" + _food.MaxAged.ToString ();
		_sellPointText.text = _food.Price.ToString ();
		// todo ロゴの設定, ボタンの表示制御

		_callBackClose = _funcClose;
		_callBackBet = _funcBet;
		_callBackSell = _funcSell;
		gameObject.SetActive (true);
	}

	public void OpenEvent(
		callBackClose _funcClose = null,
		callBackBet _funcBet = null,
		callBackSell _funcSell = null) {

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
