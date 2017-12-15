using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopupMessaegController : MonoBehaviour, IPointerClickHandler  {

	public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);
	private float duration = 0.5f;

	private GameObject msgWindow;
	private Text msgText;

	private Vector3 initScale = new Vector3 (0f, 0f, 1f);
	private Vector3 popupScale = new Vector3 (1f, 1f, 1f);
	private Vector3 popoutScale = new Vector3 (1.2f, 1.2f, 1f);


	// Use this for initialization
	void Start () {

		// 初期化
		gameObject.SetActive (false);
		msgWindow = transform.Find ("MessageWindow").gameObject;
		msgWindow.GetComponent<RectTransform> ().localScale = initScale;
		msgText = msgWindow.transform.Find ("Message").gameObject.GetComponent<Text> ();
	}

    public delegate void onConfirm();
    onConfirm _onConfirm;
    public void Popup(string msg, onConfirm onConfirmCallback)
    {
        _onConfirm = onConfirmCallback;
        Popup(msg);
    }

    public void Popup(string msg) {

		// メッセージを更新
		msgText.text = msg;

		// ポップップ開始
		gameObject.SetActive (true);
		StartCoroutine(StartPopup());
	}

	private IEnumerator StartPopup() {
		float startTime = Time.timeSinceLevelLoad;


		while((Time.timeSinceLevelLoad - startTime) < duration){

			var diff = Time.timeSinceLevelLoad - startTime;
			var rate = diff / duration;
			var pos = animCurve.Evaluate(rate);

			// popup
			msgWindow.GetComponent<RectTransform> ().localScale = Vector3.Lerp (initScale, popupScale, pos);

			yield return 0;        // 1フレーム後、再開
		}
	}

	/*
	 *  クリックされたらpopoutする
	 */
	public void OnPointerClick(PointerEventData data) {
		// ポップアウト開始
		StartCoroutine(StartPopout());
	}

	private IEnumerator StartPopout() {
		float startTime = Time.timeSinceLevelLoad;

		while((Time.timeSinceLevelLoad - startTime) < duration/5){

			var diff = Time.timeSinceLevelLoad - startTime;
			var rate = diff / duration;
			var pos = animCurve.Evaluate(rate);

			// popout
			msgWindow.GetComponent<RectTransform> ().localScale = Vector3.Lerp (popupScale, popoutScale, pos);

			yield return 0;        // 1フレーム後、再開
		}

		while((Time.timeSinceLevelLoad - startTime) < duration){

			var diff = Time.timeSinceLevelLoad - startTime;
			var rate = diff / duration;
			var pos = animCurve.Evaluate(rate);

			// popout
			msgWindow.GetComponent<RectTransform> ().localScale = Vector3.Lerp (popupScale, initScale, pos);

			yield return 0;        // 1フレーム後、再開
		}

		gameObject.SetActive (false);
        _onConfirm?.Invoke();
        _onConfirm = null;
    }
}
