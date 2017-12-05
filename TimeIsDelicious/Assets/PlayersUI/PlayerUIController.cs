using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour {

    private GameObject scoreText;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start()
    {
        scoreText = transform.Find("Score").gameObject;
        SetScore(0);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetScore(int score)
    {
        scoreText.GetComponent<Text>().text = score.ToString();
    }

    public void ChangePosision(int posision)
    {
        animator.SetInteger("Order", posision);
        animator.SetTrigger("ChangeOrder");
    }
}
