using RuleManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Specialized;

// FoodCardListのViewという扱いに変更
public class TempManager : MonoBehaviour
{
    [SerializeField]
    private GameObject cardVMPrefab;
    
    private FoodCardListVM _foodCardListVM;     // リストに対するVM
    private List<GameObject> _foodCardViewList; // 子Viewのリスト

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
                        new Vector3(-100 + 30 * i, 11, 0),
                        Quaternion.identity
                    );
                    cardview.GetComponent<CardViewModel>().nikuNo = i + 1;
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
