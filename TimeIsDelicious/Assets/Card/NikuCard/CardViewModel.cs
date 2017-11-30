using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardViewModel: MonoBehaviour {

	public GameObject cardPrefab;
	public GameObject textPrefab;

	private CardView cv;
	private GameObject agingPointText;

	public int nikuNo = 1;

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

	// 熟成度
	private int _agingPoint;
	public int agingPoint {
		set { _agingPoint = value; }
		get { return _agingPoint; }
	}

	void Start () {

		Vector3 deckPosition = GameObject.Find ("MeetDeck").transform.position;
		Debug.Log (deckPosition);

		// 肉カードを生成
		GameObject card = (GameObject)Instantiate(
			cardPrefab,
			deckPosition,
			Quaternion.identity
		);

		Debug.Log (nikuNo);
		string nikuImgName = "niku_" + nikuNo.ToString("D3");
		Texture nikuTexture = (Texture)Resources.Load (nikuImgName);
		card.GetComponent<Renderer> ().material.SetTexture("_FrontTex", nikuTexture);

		cv = card.GetComponent<CardView> ();
		cv.vm = this;
		cv.onClick = this.OnClick;

		state = Status.Init; // 初期状態
		cv.Deal(transform.position);

		// 熟成度用テキストオブジェクトを生成
		agingPointText = (GameObject)Instantiate(
			textPrefab,
			transform.position,
			Quaternion.identity
		);
		agingPointText.GetComponent<TextMesh> ().text = agingPoint.ToString();
	}

	public void OnClick() {
		Debug.Log("click card from view:" + state);

		if (state == Status.Index) {
			agingPoint++;
			agingPointText.GetComponent<TextMesh> ().text = agingPoint.ToString();

			agingPointText.SetActive (false);
			cv.ShowDetail ();
		} else if (state == Status.Detail) {
			state = Status.Animating;
			cv.ReturnIndex ();
			agingPointText.SetActive (true);
		}

	}
	/*
	private UnityEvent onTap;
	private string status = "init";

	// Use this for initialization
	void Start() {
		if (onTap == null) {
			onTap = new UnityEvent ();
		}
			
		UnityAction onTapAction = ShowDetail;
		onTap.AddListener(onTapAction);

		initScale = transform.localScale;

		initPosition = transform.position;
		//StartCoroutine(DealCardAnimation(initPosition));
	}



	public void setStatus(string _status) {
		status = _status;
	}

	public string getStatus() {
		return status;
	}
 
	// ----- ここから下はview ? 
	/*

	//  配る
	private IEnumerator DealCardAnimation (Vector3 toPosition) {

		startTime = Time.timeSinceLevelLoad;
		startPosition = transform.position;
		float duration = 1.0f;    // スライド時間（秒）
		float minAngle = 0.0F;
		float maxAngle = 180.0F;

		while((Time.time - startTime) < duration){

			var diff = Time.timeSinceLevelLoad - startTime;
			var rate = diff / time;
			var pos = animCurve.Evaluate(rate);

			// 移動
			transform.position = Vector3.Lerp (startPosition, toPosition, pos);

			// 回転
			float angle = Mathf.LerpAngle(minAngle, maxAngle, pos);
			transform.eulerAngles = new Vector3(0, 0, angle);

			yield return 0;        // 1フレーム後、再開
		}

		transform.position = toPosition;
		setStatus("dealed");
	}
	*/
}
