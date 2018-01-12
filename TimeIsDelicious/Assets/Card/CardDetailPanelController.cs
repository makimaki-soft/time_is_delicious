using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;

public class CardDetailPanelController : MonoBehaviour, IPointerClickHandler {

	public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);
	private float duration = 0.5f;

	private Image _cardImage;
	private Text _cardName;
	private Text _cardDes;
	private Text _agingPointText;
	private Text _sellPointText;
	private GameObject[] _logos;
	private GameObject _betButton;
	private GameObject _sellButton;

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

		_logos = new GameObject[2];
		_logos[0] = basePanel.transform.Find ("Logo1").gameObject;
		_logos[1] = basePanel.transform.Find ("Logo2").gameObject;
		_logos[0].SetActive (false);
		_logos[1].SetActive (false);

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

    public IObservable<Unit> OnCloseAsObservable
    {
        get { return onCloseSubject; }
    }
    private Subject<UniRx.Unit> onCloseSubject = new Subject<Unit>();

    public IObservable<Unit> OnBetButtonClickAsObservable
    {
        get { return onBetButtonClickSubject; }
    }
    private Subject<UniRx.Unit> onBetButtonClickSubject = new Subject<Unit>();

    public IObservable<Unit> OnSellButtonClickAsObservable
    {
        get { return onSellButtonClickSubject; }
    }
    private Subject<UniRx.Unit> onSellButtonClickSubject = new Subject<Unit>();

    public void OpenNiku(CardViewModel.CardMeta _food,
                         int mode, // 0:bet  1:sell
                         string CurrentPlayerName) {

		Clear ();

		string nikuImgName = "Niku/card" +  _food.ID.ToString();
		Sprite image = Resources.Load<Sprite> (nikuImgName);
		_cardImage.sprite = image;

		_cardName.text = _food.Name;
		_cardDes.text = _food.Description;

		_agingPointText.text = _food.Aged.ToString () + "/" + _food.MaxAged.ToString ();
		_sellPointText.text = _food.Price.ToString () + "G";

		IReadOnlyList<string> names = _food.NamesWhoBet;
        MakiMaki.Logger.Debug("Betting players is " + names.Count);

		// ロゴ
		for (int i = 0; i < names.Count; i++) {
			_logos [i].GetComponent<Image> ().sprite = getLogoSprite (names [i]);
			_logos [i].SetActive (true);
		}

        // ボタン制御
        //   Bet
        if (_food.CanBet
                && mode == 0
				&& !ExistMyLogo(names, CurrentPlayerName)) {
			_betButton.SetActive (true);
		}

		// Sell
		if (names.Count > 0 
                && mode == 1
				&& ExistMyLogo(names, CurrentPlayerName)) {
			_sellButton.SetActive (true);
		}

		gameObject.SetActive (true);
	}

	public void OpenEvent(
		int _eventID
	) {

		Clear ();

        string imageName = "Event/event" + _eventID.ToString ();
		Sprite image = Resources.Load<Sprite> (imageName);
		_cardImage.sprite = image;

		_cardName.text = "";
		_cardDes.text = "";

		_agingPointText.text = "";
		_sellPointText.text = "";

		gameObject.SetActive (true);
	}

	public void Close() {
		gameObject.SetActive (false);
        onCloseSubject.OnNext(Unit.Default);
	}

	public void OnClickBet() {
		gameObject.SetActive (false);
        onBetButtonClickSubject.OnNext(Unit.Default);
	}

	public void OnClickSell() {
		gameObject.SetActive (false);
        onSellButtonClickSubject.OnNext(Unit.Default);
	}

	private Sprite getLogoSprite(string _pName) {

		string imageName = "";
		switch (_pName) {
		case "鈴木精肉店":
			imageName = "logo/szks";
			break;
		case "マザーミート":
			imageName = "logo/mm";
			break;
		case "王丸農場":
			imageName = "logo/ohmaru";
			break;
		case "Chouette":
			imageName = "logo/chouette";
			break;
		}

		return Resources.Load<Sprite> (imageName);
	}

	private void Clear() {
		_logos[0].SetActive (false);
		_logos[1].SetActive (false);

		_betButton.SetActive (false);
		_sellButton.SetActive (false);
	}

	private bool ExistMyLogo(IReadOnlyList<string> _pNames, string _pName) {
		foreach(string name in _pNames){
			if (name == _pName) {
				return true;
			}
		}
		return false;
	}
}
