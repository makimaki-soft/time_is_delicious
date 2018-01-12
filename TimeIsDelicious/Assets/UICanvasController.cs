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
        MakiMaki.Logger.Debug("OnExitButtonClick");
        exitConfirmPanel.SetActive(true);
    }

    public void OnExitConfirmOKButtonClick()
    {
        MakiMaki.Logger.Debug("OnExitConfirmOKButtonClick");
		Application.LoadLevel ("Title");
    }

    public void OnExitConfirmNGButtonClick()
    {
        MakiMaki.Logger.Debug("OnExitConfirmNGButtonClick");
        exitConfirmPanel.SetActive(false);
    }
}
