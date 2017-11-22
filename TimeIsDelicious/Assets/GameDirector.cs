using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class GameDirector : MonoBehaviour, INotifyPropertyChanged
{
    private const int MaxNumberOfPlayers = 4;

    private GameObject scoreTextUI;
    private GameObject systemMenuButton;
    private GameObject playerScoreUI;

    [SerializeField]
    private GameObject playerUIPrefab;
    private List<GameObject> playerUIs;

    private int numberOfPlayers = 3;

    // Use this for initialization
    void Start () {
        scoreTextUI = GameObject.Find("Score");
        playerScoreUI = GameObject.Find("PlayerUI");

        //playerUIs = new List<GameObject>(MaxNumberOfPlayers);
        //for (int i=0; i<numberOfPlayers; i++)
        //{
        //    var ui = Instantiate(playerUIPrefab);
        //    ui.transform.parent = GameObject.Find("UICanvas").transform;
        //
        //    var recttrans = ui.GetComponent<RectTransform>();
        //    recttrans.localScale = new Vector3(2, 2, 1);
        //    recttrans.anchoredPosition = new Vector2(0, i * recttrans.sizeDelta.y * recttrans.localScale.y);
        //    playerUIs.Add(ui);
        //}
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private int count = 0;
    public int Count
    {
        get { return count; }
        set
        {
            if (Equals(count, value)) return;

            count = value;
            OnPropertyChanged("Count");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string name)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged(this, new PropertyChangedEventArgs(name));
    }


    public void Command(string message)
    {
        Count = Count + 1;
    }
}
