using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController2 : MonoBehaviour {

	public GameObject cardPrefab;

	[SerializeField]
	public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);
	public float duration = 1.0f;    // スライド時間（秒）

	[SerializeField, Range(0, 10)]
	float time = 1;

	[SerializeField]
	Vector3	endPosition;

	private float startTime;
	private Vector3 startPosition;

	float minAngle = 0.0F;
	float maxAngle = 180.0F;

	public void DealCard () {
		GameObject card = (GameObject)Instantiate(
			cardPrefab,
			new Vector3(20, 10, 0),
			Quaternion.identity
		);

		StartCoroutine(DealCardAnimation(card));
	}

	private IEnumerator DealCardAnimation (GameObject _card) {

		startTime = Time.timeSinceLevelLoad;
		startPosition = _card.transform.position;	

		while((Time.time - startTime) < duration){
			
			var diff = Time.timeSinceLevelLoad - startTime;
			var rate = diff / time;
			var pos = animCurve.Evaluate(rate);
		
			// 移動
			_card.transform.position = Vector3.Lerp (startPosition, endPosition, pos);

			// 回転
			float angle = Mathf.LerpAngle(minAngle, maxAngle, pos);
			_card.transform.eulerAngles = new Vector3(0, 0, angle);

			yield return 0;        // 1フレーム後、再開
		}

		_card.transform.position = endPosition;
	}
}
