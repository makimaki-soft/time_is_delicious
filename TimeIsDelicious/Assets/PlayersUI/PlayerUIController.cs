using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

// Player UIのViewという扱い
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
        scoreText.GetComponent<Text>().text = "0";
    }

    private PlayerVM _playerVM;
    public void setViewModel(PlayerVM model)
    {
        _playerVM = model;
        _playerVM.PropertyChanged += _playerVM_PropertyChanged; ;
    }

    private void _playerVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var player = (PlayerVM)sender;
        switch(e.PropertyName)
        {
            case "TotalEarned":
                scoreText.GetComponent<Text>().text = player.TotalEarned.ToString();
                break;
        }
    }

    public void ChangePosision(int posision)
    {
        animator.SetInteger("Order", posision);
        animator.SetTrigger("ChangeOrder");
    }
}
