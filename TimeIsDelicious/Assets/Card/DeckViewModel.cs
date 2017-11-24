using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeckViewModel : MonoBehaviour {

	public GameObject cardPrefab;

	private UnityEvent onTap;
	private string status = "init";
	private int dealCardNum = 0;
	private int DEAL_CARD_MAX_NUM = 3;
	private Vector3[] positions;

	// Use this for initialization
	void Start() {
		if (onTap == null) {
			onTap = new UnityEvent ();
		}

		UnityAction onTapAction = DealCard;
		onTap.AddListener(onTapAction);

		positions = new Vector3[DEAL_CARD_MAX_NUM];
		for(int i=0; i < positions.Length; i++) {
			positions[i] = new Vector3(i*20, 11, -15);
		}
	}

	void OnMouseDown() {
		Debug.Log("on mouse down");
		if(onTap != null && dealCardNum < DEAL_CARD_MAX_NUM) {
			dealCardNum++;
			onTap.Invoke();
		}
	}

	public void DealCard() {
		GameObject card = (GameObject)Instantiate(
			cardPrefab,
			transform.position,
			Quaternion.identity
		);
		card.name = cardPrefab.name;

		card.GetComponent<CardViewModel>().initPosition = positions[dealCardNum-1];
	}
}
