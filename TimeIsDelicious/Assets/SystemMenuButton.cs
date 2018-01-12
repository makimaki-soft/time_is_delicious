using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemMenuButton : MonoBehaviour {

    private Animator animator;
    private const int BaseLayerIndex = 0;
    private int currentStateHash;

    private Dictionary<string, int> animationStateHash;

    private GameDirector gameDirector;
    private GameObject exitButton;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();

        animationStateHash = new Dictionary<string, int>();
        animationStateHash["WaitingState"] = Animator.StringToHash("BaseLayer.WaitingState");
        animationStateHash["Normal"] = Animator.StringToHash("BaseLayer.Normal");
        animationStateHash["Pressed"] = Animator.StringToHash("BaseLayer.Pressed");

        currentStateHash = animationStateHash["Normal"];

        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();

        exitButton = transform.Find("ExitButton").gameObject;
        exitButton.SetActive(false);
    }

    void Update()
    {

        var newStateHash = animator.GetCurrentAnimatorStateInfo(BaseLayerIndex).fullPathHash;
        if(newStateHash != currentStateHash)
        {
            MakiMaki.Logger.Debug("State Changed");
            if(newStateHash == animationStateHash["WaitingState"])
            {
                exitButton.SetActive(true);
            }
            else
            {
                exitButton.SetActive(false);
            }
            currentStateHash = newStateHash;
        }

    }

    public void OnClick()
    {
        MakiMaki.Logger.Debug("Clicked/SystemMenuButton");
        var info = animator.GetCurrentAnimatorStateInfo(BaseLayerIndex);
        if (info.fullPathHash == animationStateHash["WaitingState"])
        {
            MakiMaki.Logger.Debug("Clicked/OnWaitingState");
            //gameDirector.Command("Clicked/OnWaitingState");
        }
    }
}
