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

    [SerializeField]
    private EventCardController eventCardController;

    private MainModel _singletonMainModel;

    private ObservableCollection<FoodCardVM> _currentFoodCardsVM;
    public ObservableCollection<FoodCardVM> CurrentFoodCardsVM
    {
        get { return _currentFoodCardsVM; }
    }

	// Use this for initialization
	void Start () {
        _currentFoodCardsVM = new ObservableCollection<FoodCardVM>();
        _singletonMainModel = MainModel.Instance;
        // CurrentFoodCardsプロパティ全体を公開してしまうかは悩みどころ。
        _singletonMainModel.CurrentFoodCards.ObserveAdd().Subscribe(item =>
        {
            var foodCardVM = new FoodCardVM((FoodCard)item.Value);
            _currentFoodCardsVM.Add(foodCardVM);
            foodCardFactory.CreateFoodCard(foodCardVM);
        });

        _singletonMainModel.CurrentFoodCards.ObserveRemove().Subscribe(item =>
        {
            var removedItem = _currentFoodCardsVM.FirstOrDefault(vm => vm.ID == ((FoodCard)item.Value).ID);
            if (removedItem != null)
            {
                _currentFoodCardsVM.Remove(removedItem);
                removedItem.Reset();
                foodCardFactory.RemoveFoodCard(removedItem);
            }
        });

        _singletonMainModel.CurrentEventCard.Subscribe(card=>
        {
            if(card==null)
            {
                return;
            }

            eventCardController.SetEventValues(
                card.ID,
                card.Name,
                card.Description,
                card.Weather[RuleManager.EventType.Temperature],
                card.Weather[RuleManager.EventType.Humid],
                card.Weather[RuleManager.EventType.Wind]);
        });
	}
}
