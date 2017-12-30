using RuleManager;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UniRx;

public class FoodCardVM : VMBase {


    public int ID { get; private set; }
    public IReadOnlyList<PriceTable> PriceTable { get; }
    public IReadOnlyList<CharactorTable> CharactorTable { get; }

    private FoodCard _foodCardModel;
    public FoodCardVM(FoodCard model)
    {
        _foodCardModel = model;
        this.ID = model.ID;
        PriceTable = model.PriceTable;
        CharactorTable = model.CharactorTable;
    }

    private CardViewModel cardView;
    public CardViewModel CardView
    {
        set
        {
            cardView = value;
            AgedDisposable = _foodCardModel.Aged
                                           .Where(aged=>aged<=_foodCardModel.MaxAged)
                                           .Subscribe(aged => cardView.UpdateAgedPont(aged));

            PriceDisposable = _foodCardModel.Price
                                            .Subscribe(price => cardView.UpdateSellPont(price));

            RottenDisposable = _foodCardModel.Rotten
                                             .Where(rotten=>rotten)
                                             .Subscribe(rotten => {
                cardView.UpdateAgedPont(null);
                cardView.RunPoisonEffect();
            });

            cardView.SetID(_foodCardModel.ID);

            cardView.OnClickAsObservable
                    // .First()
                    .Subscribe(_=>
            {
                var meta = new CardViewModel.CardMeta();
                meta.Aged = _foodCardModel.Aged.Value;
                meta.CanBet = _foodCardModel.CanBet;
                meta.Description = _foodCardModel.Description;
                meta.ID = _foodCardModel.ID;
                meta.MaxAged = _foodCardModel.MaxAged;
                meta.Name = _foodCardModel.Name;
                meta.NamesWhoBet = _foodCardModel.NamesWhoBet;
                meta.Price = _foodCardModel.Price.Value;
                cardView.ShowDetail(meta);
            });
        }
        get { return cardView; }
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
