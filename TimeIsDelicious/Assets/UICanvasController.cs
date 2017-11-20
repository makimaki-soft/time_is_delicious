using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasController : MonoBehaviour
{

    private GameObject exitConfirmPanel;

    // Use this for initialization
    void Start()
    {
        exitConfirmPanel = GameObject.Find("ExitConfirmPanel");
        exitConfirmPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnExitButtonClick()
    {
        Debug.Log("OnExitButtonClick");
        exitConfirmPanel.SetActive(true);
    }

    public void OnExitConfirmOKButtonClick()
    {
        Debug.Log("OnExitConfirmOKButtonClick");
        // exitConfirmPanel.SetActive(false);
    }

    public void OnExitConfirmNGButtonClick()
    {
        Debug.Log("OnExitConfirmNGButtonClick");
        exitConfirmPanel.SetActive(false);
    }
}
