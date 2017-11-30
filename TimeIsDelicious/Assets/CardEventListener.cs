using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEventListener : MonoBehaviour {

	[SerializeField]
	public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);
	public float duration = 1.0f;    // スライド時間（秒）

	[SerializeField, Range(0, 10)]
	float time = 1;

	[SerializeField]
	Vector3	endPosition;

	private float startTime;
	private Vector3 startPosition;

	
	public void ShowDetail() {
		Debug.Log("show detail");
		StartCoroutine(ShowDetailAnimation());
	}

	private IEnumerator ShowDetailAnimation () {

		startTime = Time.timeSinceLevelLoad;
		startPosition = transform.position;	

		while((Time.time - startTime) < duration){

			var diff = Time.timeSinceLevelLoad - startTime;
			var rate = diff / time;
			var pos = animCurve.Evaluate(rate);

			// 移動
			transform.position = Vector3.Lerp (startPosition, endPosition, pos);

			// 回転
			//float angle = Mathf.LerpAngle(minAngle, maxAngle, pos);
			//_card.transform.eulerAngles = new Vector3(0, 0, angle);

			yield return 0;        // 1フレーム後、再開
		}

		transform.position = endPosition;
	}
}
