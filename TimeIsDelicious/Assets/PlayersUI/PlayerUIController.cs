﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

// Player UIのViewという扱い
public class PlayerUIController : MonoBehaviour {

    private GameObject scoreText;
    private GameObject CharactorImage;
    private Animator animator;
    public int PlayerID { get; private set; }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start()
    {
        scoreText = transform.Find("Score").gameObject;
        scoreText.GetComponent<Text>().text = "0";

        CharactorImage = transform.Find("CharaImage").gameObject;
    }

    public void setViewModel(PlayerVM model)
    {
        PlayerID = model.ID;
    }

    public void Sadden()
    {
        CharactorImage.GetComponent<Faces>().Sad(2);
    }

    public void UpdateTotalEarned(int totalEarned)
    {
        CharactorImage.GetComponent<Faces>().Smile(2); // 得点が上がったら笑う
        StartCoroutine(CountUp(totalEarned));
    }

    // ここではなく、上位(ListView)側でこれを載せている皿を移動させるべき
    public void ChangePosision(int posision, onComplete onCompleteCallback)
    {
        animator.SetInteger("Order", posision);
        animator.SetTrigger("ChangeOrder");

        // アニメーション完了後にCallbackを実行
        StartCoroutine(WaitForAnimationComplete(onCompleteCallback, posision));
    }

    public delegate void onComplete(string msg);
    private IEnumerator WaitForAnimationComplete(onComplete callback, int position)
    {
        yield return new WaitUntil(() => {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
        });

        callback(position.ToString());
    }

	// スコアを1づつ増やす
	private IEnumerator CountUp(int newScore) {
		Debug.Log ("count up: " + newScore);

		int oldScore = int.Parse (scoreText.GetComponent<Text> ().text);

        float duration = 2f;
        float interval = Mathf.Min( duration / (newScore - oldScore), 0.05f);

        int tmpScore = oldScore;
		while(tmpScore != newScore)
        {
			scoreText.GetComponent<Text> ().text = tmpScore++.ToString ();
			yield return new WaitForSeconds (interval);
		}

        scoreText.GetComponent<Text>().text = newScore.ToString();
    }
}
