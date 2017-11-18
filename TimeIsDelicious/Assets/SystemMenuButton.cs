using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemMenuButton : MonoBehaviour {

    private Animator animator;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	//void Update () {
		
	//}

    

    public void OnClick()
    {
        Debug.Log("Clicked/SystemMenuButton");
        //animator.SetBool("IsSystemMenuButtonSelected", true);
    }
}
