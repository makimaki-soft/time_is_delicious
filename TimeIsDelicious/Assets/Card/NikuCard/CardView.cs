using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CardView : MonoBehaviour, IPointerClickHandler {

	public UnityAction onClick;
	public CardViewModel vm;

	private Vector3 initialPosition;
	private Vector3 initialScale;
	private Vector3 initialRotation;

	private GameObject logo1;
	private GameObject logo2;

	void Start() {
		logo1 = transform.Find ("Logo1").gameObject;
		logo2 = transform.Find ("Logo2").gameObject;

		logo1.SetActive (false);
		logo2.SetActive (false);

		RemoveLogo (1);
	}

	public void OnPointerClick(PointerEventData data) {
		// クリック時の処理
		Debug.Log("click card");
		if (onClick != null) {
			onClick ();
		} else {
			Debug.Log ("not set onClick");
		}
	}


	[SerializeField]
	public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);

	[SerializeField, Range(0, 10)]
	float time = 1;

	public void Deal(Vector3 toPosition) {
		initialPosition = toPosition;
		initialRotation = transform.eulerAngles;

		StartCoroutine (DealCardAnimation (toPosition));
		vm.state = CardViewModel.Status.Index;
	}

	public void ShowDetail() {
		vm.state = CardViewModel.Status.Animating;  // 状態をアニメーション中に変更
		StartCoroutine(MoveAnimation(
			new Vector3(0, 70, -100),
			new Vector3(-45, 0, 0)
		));
		vm.state = CardViewModel.Status.Detail;
	}

	public void ReturnIndex() {
		vm.state = CardViewModel.Status.Animating;  // 状態をアニメーション中に変更
		StartCoroutine(MoveAnimation(
			initialPosition,
			initialRotation
		));
		vm.state = CardViewModel.Status.Index;
	}

	/*
	 * カードを配るアニメーション
	 */
	private IEnumerator DealCardAnimation (Vector3 toPosition) {

		float startTime = Time.timeSinceLevelLoad;
		float duration = 1.0f;    // スライド時間（秒）

		while((Time.timeSinceLevelLoad - startTime) < duration){

			var diff = Time.timeSinceLevelLoad - startTime;
			var rate = diff / time;
			var pos = animCurve.Evaluate(rate);

			// 移動
			transform.position = Vector3.Lerp (transform.position, toPosition, pos);

			yield return 0;        // 1フレーム後、再開
		}

		transform.position = toPosition;
	}

	/*
	 * カードを詳細表示したりもどしたり 
	 */
	private IEnumerator MoveAnimation (Vector3 endPosition, Vector3 endRotaion) {

		float startTime = Time.timeSinceLevelLoad;
		float duration = 1.0f;    // スライド時間（秒）

		Vector3 startPosition = transform.position;
		Vector3 startRotation = transform.eulerAngles;

		while((Time.timeSinceLevelLoad - startTime) < duration){

			var diff = Time.timeSinceLevelLoad - startTime;
			var rate = diff / time;
			var pos = animCurve.Evaluate(rate);

			// 移動
			transform.position = Vector3.Lerp (startPosition, endPosition, pos);

			// 回転
			transform.eulerAngles = Vector3.Lerp (startRotation, endRotaion, pos);

			yield return 0;        // 1フレーム後、再開
		}

		transform.position = endPosition;

	}


	public void SetLogo(int _playerId) {
		if (!logo1.activeSelf) {
			// logo1 にセット
		} else {
			// logo2 にセット
		}
	}

	public void RemoveLogo(int _playerId) {
		Texture tex1 = logo1.GetComponent<Renderer> ().material.GetTexture ("_MainTex");
		Debug.Log (tex1.name);
	}
}
