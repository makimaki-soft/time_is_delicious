using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// CardのViewという扱いに変更する。ステータス等はVMにもっていきたい
public class CardViewModel: MonoBehaviour {

	private GameDirector _gd;

	public GameObject cardPrefab;
	public GameObject poisonEffectPrefab;

	private CardView cv;
	private GameObject cardDetailPanel;

    public int ID { get; private set; }

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
        ID = model.ID;
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
				cv.UpdateAgedPontText (agingPoint.ToString ());
            }
        }
        else if (e.PropertyName == "Rotten")
        {
            if(card.Rotten)
            {
				cv.UpdateAgedPontText ("X");

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

		_gd = GameObject.Find("GameDirector").GetComponent<GameDirector>();

		Vector3 deckPosition = GameObject.Find ("MeetDeck").transform.position;
		Debug.Log (deckPosition);

		GameObject canvas = GameObject.Find ("UICanvas");
		cardDetailPanel = canvas.transform.Find ("CardDetailPanel").gameObject;
		Debug.Log ("start: " + cardDetailPanel);

		// 肉カードを生成
		GameObject card = (GameObject)Instantiate(
			cardPrefab,
			deckPosition,
			Quaternion.identity
		);

		// 表面のテクスチャを選択
		string nikuImgName = "Niku/card" +  _cardModel.ID.ToString() + "_abst";
		Texture nikuTexture = (Texture)Resources.Load (nikuImgName);
		card.GetComponent<Renderer> ().material.SetTexture("_FrontTex", nikuTexture);

		// イベントハンドラ設定
		cv = card.GetComponent<CardView> ();
		cv.vm = this;
		cv.onClick = this.OnClick;

		state = Status.Init; // 初期状態
		cv.Deal(transform.position);
	}

	public void OnClick() {
		Debug.Log("click card " + _cardModel.Name + " from view:" + state);
        foreach( var name in _cardModel.NamesWhoBet )
        {
            Debug.Log("Bet by " + name);
        }

        cardDetailPanel.GetComponent<CardDetailPanelController> ().OpenNiku (
			_cardModel,
			null,
			() => {
				cv.SetLogo(_gd.CurrentPlayerName);
				_cardModel.BetByCurrentPlayer();
			},
			() => {
				cv.RemoveLogo(_gd.CurrentPlayerName);
				_cardModel.SellByCurrentPlayer();
			}
		);
	}

	// 毒フェクトを消す
	private IEnumerator DeleteEffect (GameObject obj, int deleteTime) {
		yield return new WaitForSeconds (deleteTime);
		Destroy (obj);
	}
}
