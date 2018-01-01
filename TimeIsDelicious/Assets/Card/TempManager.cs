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

    public void CreateFoodCard(FoodCardVM item)
    {
        int i = _foodCardViewList.Count;

        var foodCardVM = (FoodCardVM)item;
        GameObject cardview = (GameObject)Instantiate(
            cardVMPrefab,
            _bamiris[i],
            Quaternion.identity
        );
        cardview.GetComponent<CardViewModel>().setViewModel(foodCardVM);
        Observable.NextFrame().Subscribe(_ =>
        {
            foodCardVM.CardView = cardview.GetComponent<CardViewModel>();
        });
        _foodCardViewList.Add(cardview);
    }

    public void RemoveFoodCard(FoodCard item)
    {
        var foodCardVM = (FoodCard)item;
        var removedItem = _foodCardViewList.FirstOrDefault(gb => gb.GetComponent<CardViewModel>().ID == foodCardVM.ID);
        if (removedItem != null)
        {
            Destroy(removedItem);
            _foodCardViewList.Remove(removedItem);
        }
    }
}
