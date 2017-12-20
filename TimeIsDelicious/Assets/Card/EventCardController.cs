using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCardController : MonoBehaviour {

	public GameObject cardVMPrefab;

	private GameObject currentEventCard;

	private GameObject cardDetailPanel;

	// call back
	public delegate void callBackClose();
	private callBackClose _callBackClose;

	// Use this for initialization
	void Start () {
		GameObject canvas = GameObject.Find ("UICanvas");
		cardDetailPanel = canvas.transform.Find ("CardDetailPanel").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// todo event card vm をもらう？
	public void DrawEventCard(callBackClose _funcClose = null) {

		_callBackClose = _funcClose;

		if (currentEventCard != null) {
			// to do ちょっとまってから破棄
			Destroy (currentEventCard);
		}

		GameObject cardvm = (GameObject)Instantiate (
			cardVMPrefab,
			new Vector3 (0, 12, -60),
			Quaternion.identity
		);
		currentEventCard = cardvm;
		cardvm.GetComponent<EventCardViewModel> ().eventNo= 1;

		StartCoroutine (OpenEventDetail (_funcClose));
	}

	// カード配るアニメーションまち
	// todo ちゃんと終了みる？
	private IEnumerator OpenEventDetail(callBackClose _funcClose = null) {

		yield return new WaitForSeconds(1.0f);

		cardDetailPanel.GetComponent<CardDetailPanelController> ().OpenEvent (
			() => {
				_funcClose?.Invoke();
			});
	}
}
