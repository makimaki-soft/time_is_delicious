using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.localScale = new Vector3 (-1, 1, 1);
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt (Camera.main.transform);
	}
}
