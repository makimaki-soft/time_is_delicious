using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour {

    [SerializeField]
    private int score;
    private GameObject scoreText;
    private GameDirector gameDirector;

    // Use this for initialization
    void Start()
    {
        score = 0;
        scoreText = transform.Find("ScoreText").gameObject;
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();

        // Binding 
        gameDirector.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
        {
            var p = (GameDirector)sender;
            switch (e.PropertyName)
            {
                case "Count":
                    score = p.Count;
                    break;
            };
        };
    }
	
	// Update is called once per frame
	void Update () {
        scoreText.GetComponent<Text>().text = score.ToString();
    }
}
