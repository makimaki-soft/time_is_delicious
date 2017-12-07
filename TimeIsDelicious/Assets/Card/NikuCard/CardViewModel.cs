﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// CardのViewという扱いに変更する。ステータス等はVMにもっていきたい
public class CardViewModel: MonoBehaviour {

	public GameObject cardPrefab;
	public GameObject textPrefab;
	public GameObject poisonEffectPrefab;

	private CardView cv;
	private GameObject agingPointText;

	public int nikuNo = 1;

	// ステータス
	public enum Status {
		Init,     // 配布直後
		Index,    // 机の上  
		Detail,   // 詳細表示
		Animating // アニメーション中
	}
	private Status _state;
	public Status state {
		set { _state = value;}
		get { return _state;}
	}

	// 熟成度
	private int _agingPoint;
	public int agingPoint {
		set { _agingPoint = value; }
		get { return _agingPoint; }
	}

    private FoodCardVM _cardModel;
    public void setViewModel(FoodCardVM model )
    {
        _cardModel = model;
        _cardModel.PropertyChanged += _cardModel_PropertyChanged;
    }

    private void _cardModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var card = sender as FoodCardVM;
        if (e.PropertyName == "Aged")
        {
            if (!card.Rotten)
            {
                agingPoint = card.Aged;
                agingPointText.GetComponent<TextMesh>().text = agingPoint.ToString();
            }
        }
        else if (e.PropertyName == "Rotten")
        {
            if(card.Rotten)
            {
                agingPointText.GetComponent<TextMesh>().text = "×";

				// 毒フェクト
				// todo: サイズ調整
				GameObject effect = Instantiate (
					poisonEffectPrefab,
					transform.position,
					Quaternion.identity
				);
				StartCoroutine (DeleteEffect (effect, 1));
            }
        }
    }

    void Start () {

		Vector3 deckPosition = GameObject.Find ("MeetDeck").transform.position;
		Debug.Log (deckPosition);

		// 肉カードを生成
		GameObject card = (GameObject)Instantiate(
			cardPrefab,
			deckPosition,
			Quaternion.identity
		);

		Debug.Log (nikuNo);
		string nikuImgName = "niku_" + nikuNo.ToString("D3");
		Texture nikuTexture = (Texture)Resources.Load (nikuImgName);
		card.GetComponent<Renderer> ().material.SetTexture("_FrontTex", nikuTexture);

		cv = card.GetComponent<CardView> ();
		cv.vm = this;
		cv.onClick = this.OnClick;

		state = Status.Init; // 初期状態
		cv.Deal(transform.position);

		// 熟成度用テキストオブジェクトを生成
		agingPointText = (GameObject)Instantiate(
			textPrefab,
			transform.position,
			Quaternion.identity
		);
		agingPointText.GetComponent<TextMesh> ().text = agingPoint.ToString();
	}

	public void OnClick() {
		Debug.Log("click card from view:" + state);

		if (state == Status.Index) {
			agingPoint++;
			agingPointText.GetComponent<TextMesh> ().text = agingPoint.ToString();

			agingPointText.SetActive (false);
			cv.ShowDetail ();
		} else if (state == Status.Detail) {
			state = Status.Animating;
			cv.ReturnIndex ();
			agingPointText.SetActive (true);
		}

	}

	// 毒フェクトを消す
	private IEnumerator DeleteEffect (GameObject obj, int deleteTime) {
		yield return new WaitForSeconds (deleteTime);
		Destroy (obj);
	}
}
