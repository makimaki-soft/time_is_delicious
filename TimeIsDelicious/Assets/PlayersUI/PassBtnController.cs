using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.UI;

public class PassBtnController : MonoBehaviour {

    public Button passButton;

	// Use this for initialization
	void Start () {
        passButton.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void SetActive(bool active)
    {
        passButton.gameObject.SetActive(active);
    }

    public IObservable<Unit> OnClickAsObservable
    {
        get { return passButton.OnClickAsObservable(); }
    }
}
