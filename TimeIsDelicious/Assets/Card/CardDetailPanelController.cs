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

	public delegate void callBack();
	private callBack _callBack;

	// Use this for initialization
	void Start () {
		// 初期化
		gameObject.SetActive (false);

		GameObject basePanel = transform.Find ("BasePanel").gameObject;
		_cardImage = basePanel.transform.Find ("CardImage").GetComponent<Image> ();

		_logo1 = basePanel.transform.Find ("Logo1").GetComponent<Image> ();
		_logo2 = basePanel.transform.Find ("Logo2").GetComponent<Image> ();
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

	public void Open(callBack _func) {

		// todo データをセットする

		_callBack = _func;
		gameObject.SetActive (true);
	}

	public void Close() {
		gameObject.SetActive (false);

		_callBack?.Invoke();
		_callBack = null;
	}
}
