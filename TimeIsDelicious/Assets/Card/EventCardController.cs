using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class EventCardController : MonoBehaviour {

	public GameObject cardVMPrefab;

	private GameObject currentEventCard;

	private GameObject cardDetailPanel;

	// call back
	public delegate void callBackClose();
	private callBackClose _callBackClose;

    public class EventValues
    {
        public int ID;
        public string Name;
        public string Description;
        public int Temperature;
        public int Humidity;
        public int Wind;
    };

    // Use this for initialization
    void Start () {
		GameObject canvas = GameObject.Find ("UICanvas");
		cardDetailPanel = canvas.transform.Find ("CardDetailPanel").gameObject;
    }

    // Update is called once per frame
    void Update () {
		
	}

	// todo event card vm をもらう？
    public IObservable<Unit> DrawEventCard(EventValues eventValues) {
        
		if (currentEventCard != null) {
			// to do ちょっとまってから破棄
			Destroy (currentEventCard);
		}

		GameObject cardvm = (GameObject)Instantiate (
			cardVMPrefab,
			new Vector3 (50, 12, 0),
			Quaternion.identity
		);
		currentEventCard = cardvm;
        MakiMaki.Logger.Debug("現在のイベント/気温" + eventValues.Temperature + "/湿度" + eventValues.Humidity + "/風" + eventValues.Wind);
        MakiMaki.Logger.Debug ("id: " + eventValues.ID + "Name" + eventValues.Name);
		// StartCoroutine (OpenEventDetail ());

        var hotStream = Observable.FromCoroutine(OpenEventDetail).Publish().RefCount();
        hotStream.Subscribe();
        return hotStream;
	}

	// カード配るアニメーションまち
	// todo ちゃんと終了みる？
	private IEnumerator OpenEventDetail() {
		yield return new WaitForSeconds(1.0f);
	}
}
