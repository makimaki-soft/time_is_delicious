using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventDeck : MonoBehaviour, IPointerClickHandler {

	public GameObject cardVMPrefab;

	private GameObject currentEventCard;

	// Use this for initialization
	void Start () {
		i = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	private int i;
	public void OnPointerClick(PointerEventData data) {
		// クリック時の処理
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
		i++;
		cardvm.GetComponent<EventCardViewModel> ().eventNo= i % 2 + 1;
	}
}
