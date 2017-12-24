using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndController : MonoBehaviour {


	private PermanentObj _pObj;

	// Use this for initialization
	void Start () {
		_pObj = GameObject.Find ("PermanentObj").GetComponent<PermanentObj> ();

		GameObject.Find("CharaThumbnail").ga
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ReturnToStart () {
		Debug.Log ("return to start");
		Application.LoadLevel ("Main");
	}
}
