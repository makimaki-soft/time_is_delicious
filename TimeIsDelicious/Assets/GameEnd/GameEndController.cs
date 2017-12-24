using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndController : MonoBehaviour {

	public GameObject Background;
	public Image winPlayerImage;
	public Text winMessage;

	private PermanentObj _pObj;

	// Use this for initialization
	void Start () {
		_pObj = GameObject.Find ("PermanentObj")?.GetComponent<PermanentObj> ();
		GameObject[] thumbnails = GameObject.FindGameObjectsWithTag ("CharaThumbnail");
		GameObject[] scores = GameObject.FindGameObjectsWithTag ("Score");

		Debug.Log ("thumbnails " + thumbnails.Length);
		Debug.Log ("scores " + scores.Length);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ReturnToStart () {
		Debug.Log ("return to start");
		Application.LoadLevel ("Main");
	}
}
