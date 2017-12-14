using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleController : MonoBehaviour, IPointerClickHandler {


	// Use this for initialization
	void Start () {
		gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {

	}

	// Title panelを表示する
	public void Show() {
		gameObject.SetActive (true);
	}

	// クリックされたらSettingシーンへ
	public void OnPointerClick(PointerEventData data) {
		Application.LoadLevel ("Setting");
	}
}
