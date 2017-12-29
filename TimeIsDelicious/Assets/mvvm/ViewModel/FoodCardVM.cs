using RuleManager;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UniRx;

public class FoodCardVM : VMBase {

    private readonly int _id;
    public int ID
    {
        get { return _id; }
    }

    private readonly string _name;
    public string Name
    {
        get { return _name; }
    }

    private readonly string _description;
    public string Description
    {
        get { return _description; }
    }


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

    public bool CanBet
    {
        get { return _foodCardModel.CanBet; }
    }

    public IReadOnlyList<string> NamesWhoBet
    {
        get { return _foodCardModel.NamesWhoBet; }
    }

    public IReadOnlyList<PriceTable> PriceTable { get; }
    public IReadOnlyList<CharactorTable> CharactorTable { get; }

    private FoodCard _foodCardModel;
    public FoodCardVM(FoodCard model)
    {
        _id = model.ID;
        _name = model.Name;
        _description = model.Description;
        _aged = model.Aged.Value;
        _maxAged = model.MaxAged;
        _price = model.Price.Value;
        _rotten = model.Rotten.Value;
        _foodCardModel = model;
        PriceTable = model.PriceTable;
        CharactorTable = model.CharactorTable;

        AgedDisposable = model.Aged.Subscribe(aged => Aged = aged);
        PriceDisposable = model.Price.Subscribe(price => Price = price);
        RottenDisposable = model.Rotten.Subscribe(rotten => Rotten = rotten);
    }

    private System.IDisposable AgedDisposable;
    private System.IDisposable PriceDisposable;
    private System.IDisposable RottenDisposable;

    public void Reset()
    {
        AgedDisposable.Dispose();
        PriceDisposable.Dispose();
        RottenDisposable.Dispose();
    }

    public void BetByCurrentPlayer()
    {
        MainModel.Instance.BetFood(_foodCardModel);
    }

    public void SellByCurrentPlayer()
    {
        MainModel.Instance.SellFood(_foodCardModel);
    }
}
