using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour {

	public GameObject poisonEffectPrefab1;
	public GameObject poisonEffectPrefab2;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddPoisonEffect1 () {
		GameObject effect = Instantiate (
			poisonEffectPrefab1,
			transform.position,
			Quaternion.identity
		);
		StartCoroutine (DeleteEffect (effect, 3));
	}

	public void AddPoisonEffect2 () {
		GameObject effect = Instantiate (
			poisonEffectPrefab2,
			transform.position,
			Quaternion.identity
		);
		StartCoroutine (DeleteEffect (effect, 1));
	}

	private IEnumerator DeleteEffect (GameObject obj, int deleteTime) {
		yield return new WaitForSeconds (deleteTime);
		Destroy (obj);
	}
}
