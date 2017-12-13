using RuleManager;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

public class FoodCardVM : VMBase {

    private int _aged;
    public int Aged
    {
        get { return _aged; }
        set
        {
            if (value != _aged)
            {
                _aged = value;
                NotifyPropertyChanged();
            }
        }
    }

    private readonly int _maxAged;
    public int MaxAged
    {
        get { return _maxAged; }
    }

    private int _price;
    public int Price
    {
        get { return _price; }
        private set
        {
            if (value != _price)
            {
                _price = value;
                NotifyPropertyChanged();
            }
        }
    }

    private bool _rotten;
    public bool Rotten
    {
        get { return _rotten; }
        private set
        {
            if (value != _rotten)
            {
                _rotten = value;
                NotifyPropertyChanged();
            }
        }
    }

    public IReadOnlyList<PriceTable> PriceTable { get; }

    private FoodCard _foodCardModel;
    public FoodCardVM(FoodCard model)
    {
        _aged = 0;
        _maxAged = 50;
        _price = 0;
        _rotten = false;
        _foodCardModel = model;
        PriceTable = model.PriceTable;

        _foodCardModel.PropertyChanged += _foodCardModel_PropertyChanged;
    }

    private void _foodCardModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var card = (FoodCard)sender;
        switch (e.PropertyName)
        {
            case "Aged":
                Aged = card.Aged;
                break;
            case "Price":
                Price = card.Price;
                break;
            case "Rotten":
                Rotten = card.Rotten;
                break;
        }
    }
}
