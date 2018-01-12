using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCardViewModel : MonoBehaviour {

	public GameObject cardPrefab;

	private GameObject card;
	private EventCardView cv;

	// ステータス
	public enum Status {
		Init,     // 配布直後
		Index,    // 机の上  
		Detail,   // 詳細表示
		Animating // アニメーション中
	}
	private Status _state;
	public Status state {
		set { _state = value;}
		get { return _state;}
	}

	void Start () {

		Vector3 deckPosition = GameObject.Find ("EventDeck").transform.position;
		MakiMaki.Logger.Debug (deckPosition);

		// カードを生成
		card = (GameObject)Instantiate(
			cardPrefab,
			deckPosition,
			Quaternion.identity
		);
			
		string eventImgName = "Event/event0";
		Texture eventTexture = (Texture)Resources.Load (eventImgName);
		card.GetComponent<Renderer> ().material.SetTexture("_FrontTex", eventTexture);

		cv = card.GetComponent<EventCardView> ();
		cv.vm = this;
		cv.onClick = this.OnClick;

		state = Status.Init; // 初期状態
		cv.Deal(transform.position);

	}

	public void OnClick() {
		MakiMaki.Logger.Debug("click card from view:" + state);

		if (state == Status.Index) {
			cv.ShowDetail ();
		} else if (state == Status.Detail) {
			state = Status.Animating;
			cv.ReturnIndex ();
		}

	}

	void OnDestroy() {
		MakiMaki.Logger.Debug("on destory");
		Destroy (card);
	}
}
