using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardViewModel : MonoBehaviour {

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

	/*
	void Update() {
		if (Input.GetMouseButtonDown (0)) {
			Debug.Log("on mouse down in update");
			if(onTap != null) {
				onTap.Invoke();
			}
		}
	}
	*/

	void OnMouseDown() {
		// なぜかうごかない
		Debug.Log("on mouse down");
		if(onTap != null) {
			onTap.Invoke();
		}
	}

	public void setStatus(string _status) {
		status = _status;
	}

	public string getStatus() {
		return status;
	}
 
	// ----- ここから下はview ? 

	[SerializeField]
	public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);

	[SerializeField, Range(0, 10)]
	float time = 1;

	private float startTime;
	public Vector3 initPosition;
	private Vector3 initScale;

	private Vector3 startPosition;
	private Vector3	endPosition;
	private Vector3 startScale;
	private Vector3 endScale;

	float minAngle;
	float maxAngle;

	public void ShowDetail() {
		Debug.Log("show detail");

		startPosition = transform.position;
		startScale = transform.localScale;

		if("dealed".Equals(getStatus())) {
			setStatus("detail");

			endPosition = new Vector3(0, 60, -85);
			endScale = startScale * 1.5f;
			minAngle = 0.0F;
			maxAngle = -45.0F;
			StartCoroutine(ShowDetailAnimation());
		}

		if("detail".Equals(getStatus())) {
			setStatus("dealed");

			endPosition = initPosition;
			endScale = initScale;
			minAngle = -45.0F;
			maxAngle = 0.0F;
			StartCoroutine(ShowDetailAnimation());
		}

	}

	// 詳細表示
	private IEnumerator ShowDetailAnimation () {

		float startTime = Time.timeSinceLevelLoad;
		float duration = 1.0f;    // スライド時間（秒）

		while((Time.time - startTime) < duration){

			var diff = Time.timeSinceLevelLoad - startTime;
			var rate = diff / time;
			var pos = animCurve.Evaluate(rate);

			// 移動
			transform.position = Vector3.Lerp (startPosition, endPosition, pos);

			// 回転
			float angle = Mathf.LerpAngle(minAngle, maxAngle, pos);
			transform.eulerAngles = new Vector3(angle, 0, 0);

			// サイズ
			transform.localScale = Vector3.Lerp(startScale, endScale, pos);

			yield return 0;        // 1フレーム後、再開
		}

		transform.position = endPosition;
	}

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
}
