using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemMenuButton : MonoBehaviour {

    private Animator animator;
    private int waitingStateHash;
    private const int BaseLayerIndex = 0;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        waitingStateHash = Animator.StringToHash("BaseLayer.WaitingState");
    }

    public void OnClick()
    {
        Debug.Log("Clicked/SystemMenuButton");
        var info = animator.GetCurrentAnimatorStateInfo(BaseLayerIndex);
        if (info.fullPathHash == waitingStateHash)
        {
            Debug.Log("Clicked/OnWaitingState");
        }
    }
}
