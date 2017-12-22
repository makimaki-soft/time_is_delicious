using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCardController : MonoBehaviour {

	public GameObject cardVMPrefab;

	private GameObject currentEventCard;

	private GameObject cardDetailPanel;

	// call back
	public delegate void callBackClose();
	private callBackClose _callBackClose;

    private EventCardVM _vm;
    class EventValues
    {
        public int Temperature;
        public int Humidity;
        public int Wind;
    };
    EventValues _eventValues;

    // Use this for initialization
    void Start () {
		GameObject canvas = GameObject.Find ("UICanvas");
		cardDetailPanel = canvas.transform.Find ("CardDetailPanel").gameObject;
        _vm = new EventCardVM();
        _eventValues = new EventValues();
        _vm.PropertyChanged += _vm_PropertyChanged;
    }

    private void _vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var vm = (EventCardVM)sender;
        switch(e.PropertyName)
        {
            case "Temperature":
                _eventValues.Temperature = vm.Temperature;
                break;
            case "Humidity":
                _eventValues.Humidity = vm.Humidity;
                break;
            case "Wind":
                _eventValues.Wind = vm.Wind;
                break;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

	// todo event card vm をもらう？
	public void DrawEventCard(callBackClose _funcClose = null) {

		_callBackClose = _funcClose;

		if (currentEventCard != null) {
			// to do ちょっとまってから破棄
			Destroy (currentEventCard);
		}

		GameObject cardvm = (GameObject)Instantiate (
			cardVMPrefab,
			new Vector3 (0, 12, -60),
			Quaternion.identity
		);
		currentEventCard = cardvm;
		cardvm.GetComponent<EventCardViewModel> ().eventNo= 1;
        Debug.Log("現在のイベント/気温" + _eventValues.Temperature + "/湿度" + _eventValues.Humidity + "/風" + _eventValues.Wind);

		StartCoroutine (OpenEventDetail (_funcClose));
	}

	// カード配るアニメーションまち
	// todo ちゃんと終了みる？
	private IEnumerator OpenEventDetail(callBackClose _funcClose = null) {

		yield return new WaitForSeconds(1.0f);

		cardDetailPanel.GetComponent<CardDetailPanelController> ().OpenEvent (
			() => {
				_funcClose?.Invoke();
			});
	}
}
