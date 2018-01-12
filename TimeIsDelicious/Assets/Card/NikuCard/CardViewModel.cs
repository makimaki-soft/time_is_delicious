﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using UniRx.Triggers;

// CardのViewという扱いに変更する。ステータス等はVMにもっていきたい
public class CardViewModel : MonoBehaviour
{
    public GameObject cardPrefab;
    public GameObject poisonEffectPrefab;

    private GameObject card;
    private CardView cv;
    private GameObject cardDetailPanel;

    public int ID { get; private set; }

    // ステータス
    public enum Status
    {
        Init,     // 配布直後
        Index,    // 机の上
        Detail,   // 詳細表示
        Animating // アニメーション中
    }
    private Status _state;
    public Status state
    {
        set { _state = value; }
        get { return _state; }
    }

    // 熟成度
    private int _agingPoint;
    public int agingPoint
    {
        set { _agingPoint = value; }
        get { return _agingPoint; }
    }

    public void UpdateAgedPont(int? aged)
    {
        var text = aged.HasValue ? aged.Value.ToString() : "✕";
        cv?.UpdateAgedPontText(text + "/50");
    }

    public void UpdateSellPont(int PricePoint)
    {
        var text = PricePoint.ToString();
        cv?.UpdateSellPontText(text);
    }


    public void RunPoisonEffect()
    {
        // 毒フェクト
        // todo: サイズ調整
        GameObject effect = Instantiate(
            poisonEffectPrefab,
            transform.position,
            Quaternion.identity
        );
        StartCoroutine(DeleteEffect(effect, 1));
    }

    void Start()
    {
        Vector3 deckPosition = GameObject.Find("MeetDeck").transform.position;
        MakiMaki.Logger.Debug(deckPosition);

        GameObject canvas = GameObject.Find("UICanvas");
        cardDetailPanel = canvas.transform.Find("CardDetailPanel").gameObject;
        MakiMaki.Logger.Debug("start: " + cardDetailPanel);

        // 肉カードを生成
        card = (GameObject)Instantiate(
            cardPrefab,
            deckPosition,
            Quaternion.identity
        );
    }

    public void SetID(int id)
    {
        this.ID = id;

        // 表面のテクスチャを選択
        //string nikuImgName = "Niku/card" +  _cardModel.ID.ToString() + "_abst";
        string nikuImgName = "Niku/card" + ID.ToString();
        Texture nikuTexture = (Texture)Resources.Load(nikuImgName);
        card.GetComponent<Renderer>().material.SetTexture("_FrontTex", nikuTexture);

        // イベントハンドラ設定
        cv = card.GetComponent<CardView>();
        cv.vm = this;
        cv.onClick = this.OnClick;

        state = Status.Init; // 初期状態
        cv.Deal(transform.position);
    }

    public void OnClick()
    {
        onClickSubject.OnNext(Unit.Default);
    }



    public IObservable<Unit> OnClickAsObservable
    { 
        get { return onClickSubject; }
    }
    private Subject<UniRx.Unit> onClickSubject = new Subject<Unit>();

    public class CardMeta
    {
        public int ID;
        public string Name;
        public string Description;
        public IReadOnlyList<string> NamesWhoBet;
        public int Aged;
        public int MaxAged;
        public int Price;
        public bool CanBet;
    }

    public void SetLogo(string Name)
    {
        cv?.SetLogo(Name);
    }

    public void RemoveLogo(string Name)
    {
        cv?.RemoveLogo(Name);
    }

	// 毒フェクトを消す
	private IEnumerator DeleteEffect (GameObject obj, int deleteTime) {
		yield return new WaitForSeconds (deleteTime);
		Destroy (obj);
	}

	void OnDestroy(){
		MakiMaki.Logger.Debug ("!!! D E S T O R O Y !!!");
        onClickSubject.OnCompleted();
		Destroy (card);
	}
}
