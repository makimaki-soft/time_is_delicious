﻿using RuleManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Specialized;
using System.Linq;

// FoodCardListのViewという扱いに変更
public class TempManager : MonoBehaviour
{
    [SerializeField]
    private GameObject cardVMPrefab;
    
    private FoodCardListVM _foodCardListVM;     // リストに対するVM
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
        _foodCardListVM = new FoodCardListVM();
        _foodCardListVM.CurrentFoodCardsVM.CollectionChanged += CurrentFoodCardsVM_CollectionChanged;
    }

    private void CurrentFoodCardsVM_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:

                foreach (var item in e.NewItems)
                {
                    int i = _foodCardViewList.Count;

                    var foodCardVM = (FoodCardVM)item;
                    GameObject cardview = (GameObject)Instantiate(
                        cardVMPrefab,
						_bamiris[i],
                        Quaternion.identity
                    );
                    cardview.GetComponent<CardViewModel>().setViewModel(foodCardVM);
                    _foodCardViewList.Add(cardview);
                }
                Debug.Log("CurrentFoodCardsVM Add");
                break;
            case NotifyCollectionChangedAction.Move:
                Debug.Log("CurrentFoodCardsVM Move");
                break;
            case NotifyCollectionChangedAction.Remove:
                Debug.Log("CurrentFoodCardsVM Remove");
                foreach (var item in e.OldItems)
                {
                    var foodCardVM = (FoodCardVM)item;
                    var removedItem = _foodCardViewList.FirstOrDefault(gb => gb.GetComponent<CardViewModel>().ID == foodCardVM.ID);
                    if(removedItem != null)
                    {
                        Destroy(removedItem);
                        _foodCardViewList.Remove(removedItem);
                    }
                }
                break;
            case NotifyCollectionChangedAction.Replace:
                Debug.Log("CurrentFoodCardsVM Replace");
                break;
            case NotifyCollectionChangedAction.Reset:
                Debug.Log("CurrentFoodCardsVM Reset");
                break;
        }
    }
}
