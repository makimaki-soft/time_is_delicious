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

    class EventValues
    {
        public int ID;
        public string Name;
        public string Description;
        public int Temperature;
        public int Humidity;
        public int Wind;
    };
    EventValues _eventValues;

    // Use this for initialization
    void Start () {
		GameObject canvas = GameObject.Find ("UICanvas");
		cardDetailPanel = canvas.transform.Find ("CardDetailPanel").gameObject;
        _eventValues = new EventValues();
    }

    public void SetEventValues(int ID, string Name, string Description, int Temperature, int Humidity,int Wind )
    {
        _eventValues.ID = ID;
        _eventValues.Name = Name;
        _eventValues.Description = Description;
        _eventValues.Temperature = Temperature;
        _eventValues.Humidity = Humidity;
        _eventValues.Wind = Wind;
    }

    // Update is called once per frame
    void Update () {
		
	}

	// todo event card vm をもらう？
    public IObservable<Unit> DrawEventCard() {
        
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
        Debug.Log("現在のイベント/気温" + _eventValues.Temperature + "/湿度" + _eventValues.Humidity + "/風" + _eventValues.Wind);
		Debug.Log ("id: " + _eventValues.ID + "Name" + _eventValues.Name);
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
