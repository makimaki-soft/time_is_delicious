using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using UniRx;

namespace RuleManager
{
    public class Player : GameComponent
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public IReactiveProperty<int> TotalEarned { get; private set; }

        private ObservableCollection<FoodCard> _bets;
        public ObservableCollection<FoodCard> Bets
        {
            get { return _bets; }
        }

        public void Bet(FoodCard card)
        {
            _bets.Add(card);
            card.SetBetPlayer(this);

            RottenDisposable = card.Rotten
                .Where(rotten => rotten==true)
                .First()
                .Subscribe(rotten=>{
                if (_bets.Contains(card))
                {
                    _bets.Remove(card);
                    card.RemoveBetPlayer(this);
                }
            });
        }

        private IDisposable RottenDisposable;

        public void Sell(FoodCard card)
        {
            if( !_bets.Any(n => n.GUID == card.GUID) )
            {
                UnityEngine.Debug.Log("持ってないカードは売れません。");
                return;
            }
            TotalEarned.Value += card.Price.Value;
            _bets.Remove(card);
            card.RemoveBetPlayer(this);

            RottenDisposable.Dispose();
        }

        public void SellAll()
        {
            var sell = _bets.ToArray();
            var sellCount = _bets.Count;

            for(int i=0; i<sellCount;i++)
            {
                Sell(sell[i]);
            }

        }

        public Player(int id, string name)
        {
            this.ID = id;
            this.Name = name;
            _bets = new ObservableCollection<FoodCard>();
            _bets.CollectionChanged += _bets_CollectionChanged;

            TotalEarned = new ReactiveProperty<int>(0); 
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
