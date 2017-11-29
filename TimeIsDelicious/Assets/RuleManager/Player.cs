using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RuleManager
{
    class Player : GameComponent
    {
        private readonly int _id;
        public int ID
        {
            get { return _id; }
        }

        private int _totalEarned;
        public int TotalEarned
        {
            get { return _totalEarned; }
            set
            {
                if (value != _totalEarned)
                {
                    _totalEarned = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private List<FoodCard> _bets;
        public List<FoodCard> Bets
        {
            get { return _bets; }
        }

        public void Bet(FoodCard card)
        {
            _bets.Add(card);
            card.PropertyChanged += OnFoodCardPropertyChanged;
        }

        public void Sell(FoodCard card)
        {
            TotalEarned += card.Price;
            _bets.Remove(card);
            card.PropertyChanged -= OnFoodCardPropertyChanged;
        }

        public void OnFoodCardPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            var card = sender as FoodCard;
            switch (e.PropertyName)
            {
                case "Rotten":
                    if (card.Rotten == true)
                    {
                        var rotten = _bets.Find(x => x.GUID == card.GUID);
                        _bets.Remove(rotten);
                        card.PropertyChanged -= OnFoodCardPropertyChanged;
                    }
                    break;
            }
        }

        public Player(int i)
        {
            _id = i;
            _bets = new List<FoodCard>();
        }
    }
}
