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
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start()
    {
        score = 0;
        scoreText = transform.Find("Score").gameObject;
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        // animator = GetComponent<Animator>();

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
    void Update()
    {
        scoreText.GetComponent<Text>().text = score.ToString();
    }

    public void AddScore()
    {
        score++;
    }

    public void ChangeOrder(int order)
    {
        animator.SetInteger("Order", order);
        animator.SetTrigger("ChangeOrder");
    }
}
