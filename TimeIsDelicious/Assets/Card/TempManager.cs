using RuleManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Specialized;
using System.Linq;
using UniRx;

// FoodCardListのViewという扱いに変更
public class TempManager : MonoBehaviour
{
    [SerializeField]
    private GameObject cardVMPrefab;
    private List<GameObject> _foodCardViewList; // 子Viewのリスト

	private Vector3[] _bamiris = new Vector3[] {
		new Vector3 (-45, 11, 0),
		new Vector3 (0, 11, 0),
		new Vector3 (-70, 11, -40),
		new Vector3 (-25, 11, -40),
		new Vector3 (20, 11, -40)
	};

    // Use this for initialization
    void Start()
    {
        _foodCardViewList = new List<GameObject>();
    }

    public CardViewModel CreateFoodCard()
    {
        int i = _foodCardViewList.Count;

        GameObject cardview = (GameObject)Instantiate(
            cardVMPrefab,
            _bamiris[i],
            Quaternion.identity
        );
        _foodCardViewList.Add(cardview);
        return cardview.GetComponent<CardViewModel>();
    }

    public void RemoveFoodCard(CardViewModel item)
    {
        var foodCardView = (CardViewModel)item;
        var removedItem = _foodCardViewList.FirstOrDefault(gb => gb.GetComponent<CardViewModel>().ID == foodCardView.ID);
        if (removedItem != null)
        {
            Destroy(removedItem);
            _foodCardViewList.Remove(removedItem);
            foodCardView = null;
        }
    }
}
