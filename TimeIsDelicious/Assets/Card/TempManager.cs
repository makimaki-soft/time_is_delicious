using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempManager : MonoBehaviour {

	public GameObject cardVMPrefab;

	// Use this for initialization
	void Start () {

		for (int i = 0; i < 5; i++) {
			GameObject cardvm = (GameObject)Instantiate (
				cardVMPrefab,
				new Vector3 (-100+30*i, 11, 0),
				Quaternion.identity
			);
			cardvm.GetComponent<CardViewModel> ().nikuNo=i+1;
		}
	}

}
