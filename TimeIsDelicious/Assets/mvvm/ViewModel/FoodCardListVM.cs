using RuleManager;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

// 肉カードVMの親VM
public class FoodCardListVM : VMBase {

    private MainModel _singletonMainModel;

    public FoodCardListVM()
    {
        _currentFoodCardsVM = new ObservableCollection<FoodCardVM>();

        _singletonMainModel = MainModel.Instance;
        // CurrentFoodCardsプロパティ全体を公開してしまうかは悩みどころ。
        _singletonMainModel.CurrentFoodCards.CollectionChanged += CurrentFoodCards_CollectionChanged;
    }

    private ObservableCollection<FoodCardVM> _currentFoodCardsVM;
    public ObservableCollection<FoodCardVM> CurrentFoodCardsVM
    {
        get { return _currentFoodCardsVM; }
    }

    private void CurrentFoodCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch(e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach( var item in e.NewItems )
                {
                    var foodCardVM = new FoodCardVM((FoodCard)item);
                    _currentFoodCardsVM.Add(foodCardVM);
                }
                UnityEngine.Debug.Log("CurrentFoodCards Add");
                break;
            case NotifyCollectionChangedAction.Move:
                UnityEngine.Debug.Log("CurrentFoodCards Move");
                break;
            case NotifyCollectionChangedAction.Remove:
                UnityEngine.Debug.Log("CurrentFoodCards Remove");
                break;
            case NotifyCollectionChangedAction.Replace:
                UnityEngine.Debug.Log("CurrentFoodCards Replace");
                break;
            case NotifyCollectionChangedAction.Reset:
                UnityEngine.Debug.Log("CurrentFoodCards Reset");
                break;
        }
    }
}
