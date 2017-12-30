using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using RuleManager;
using System.Linq;

public class MainPresenter : MonoBehaviour {


    [SerializeField]
    private TempManager foodCardFactory;

    private MainModel _singletonMainModel;

    private ObservableCollection<FoodCardVM> _currentFoodCardsVM;
    public ObservableCollection<FoodCardVM> CurrentFoodCardsVM
    {
        get { return _currentFoodCardsVM; }
    }

    private void CurrentFoodCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems)
                {
                    var foodCardVM = new FoodCardVM((FoodCard)item);
                    _currentFoodCardsVM.Add(foodCardVM);
                    foodCardFactory.CreateFoodCard(foodCardVM);
                }
                UnityEngine.Debug.Log("CurrentFoodCards Add");
                break;
            case NotifyCollectionChangedAction.Move:
                UnityEngine.Debug.Log("CurrentFoodCards Move");
                break;
            case NotifyCollectionChangedAction.Remove:
                UnityEngine.Debug.Log("CurrentFoodCards Remove");
                foreach (var item in e.OldItems)
                {
                    var removedItem = _currentFoodCardsVM.FirstOrDefault(vm => vm.ID == ((FoodCard)item).ID);
                    if (removedItem != null)
                    {
                        _currentFoodCardsVM.Remove(removedItem);
                        removedItem.Reset();
                        foodCardFactory.RemoveFoodCard(removedItem);
                    }
                }
                break;
            case NotifyCollectionChangedAction.Replace:
                UnityEngine.Debug.Log("CurrentFoodCards Replace");
                break;
            case NotifyCollectionChangedAction.Reset:
                UnityEngine.Debug.Log("CurrentFoodCards Reset");
                break;
        }
    }

	// Use this for initialization
	void Start () {
        _currentFoodCardsVM = new ObservableCollection<FoodCardVM>();
        _singletonMainModel = MainModel.Instance;
        // CurrentFoodCardsプロパティ全体を公開してしまうかは悩みどころ。
        _singletonMainModel.CurrentFoodCards.CollectionChanged += CurrentFoodCards_CollectionChanged;
	}
}
