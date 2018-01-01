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
        public ReactiveCollection<FoodCard> Bets { get; private set; }

        public void Bet(FoodCard card)
        {
            Bets.Add(card);
            card.SetBetPlayer(this);

            RottenDisposable = card.Rotten
                .Where(rotten => rotten==true)
                .First()
                .Subscribe(rotten=>{
                if (Bets.Contains(card))
                {
                    Bets.Remove(card);
                    card.RemoveBetPlayer(this);
                }
            });
        }

        private IDisposable RottenDisposable;

        public void Sell(FoodCard card)
        {
            if( !Bets.Any(n => n.GUID == card.GUID) )
            {
                UnityEngine.Debug.Log("持ってないカードは売れません。");
                return;
            }
            TotalEarned.Value += card.Price.Value;
            Bets.Remove(card);
            card.RemoveBetPlayer(this);

            RottenDisposable.Dispose();
        }

        public void SellAll()
        {
            var sell = Bets.ToArray();
            var sellCount = Bets.Count;

            for(int i=0; i<sellCount;i++)
            {
                Sell(sell[i]);
            }

        }

        public Player(int id, string name)
        {
            this.ID = id;
            this.Name = name;
            Bets = new ReactiveCollection<FoodCard>();
            TotalEarned = new ReactiveProperty<int>(0); 
        }
    }
}
