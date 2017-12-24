using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace RuleManager
{
    public class Player : GameComponent
    {
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

        private ObservableCollection<FoodCard> _bets;
        public ObservableCollection<FoodCard> Bets
        {
            get { return _bets; }
        }

        public void Bet(FoodCard card)
        {
            _bets.Add(card);
            card.SetBetPlayer(this);
            card.PropertyChanged += OnFoodCardPropertyChanged;
        }

        public void Sell(FoodCard card)
        {
            if( !_bets.Any(n => n.GUID == card.GUID) )
            {
                UnityEngine.Debug.Log("持ってないカードは売れません。");
                return;
            }
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
                    if (card.Rotten == true && _bets.Contains(card))
                    {
                        // var rotten = _bets.Find(x => x.GUID == card.GUID);
                        _bets.Remove(card);
                        card.PropertyChanged -= OnFoodCardPropertyChanged;
                    }
                    break;
            }
        }

        public Player(int id, string name)
        {
            _id = id;
            _name = name;
            _bets = new ObservableCollection<FoodCard>();
            _bets.CollectionChanged += _bets_CollectionChanged;
        }

        public delegate int BetsCountDelegate(int newCount);
        public BetsCountDelegate LookCount { get; set; }
        private void _bets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var tmp = (ObservableCollection<FoodCard>)sender;
            LookCount?.Invoke(tmp.Count);
        }
    }
}
