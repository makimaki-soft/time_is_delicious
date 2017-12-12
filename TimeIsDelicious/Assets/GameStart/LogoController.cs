using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoController : MonoBehaviour {

	public GameObject title;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void EndFadeOutAnimation () {
		title.GetComponent<TitleController> ().Show ();
	}
}
