using System.Collections.Generic;
using UniRx;

namespace RuleManager
{
    public class PriceTable
    {
        public int min;
        public int max;
        public int price;
    }

    public class CharactorTable
    {
        public EventType type;
        public int threshold;
        public int scale;
    }

    public class FoodCard : GameComponent, ICard
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int MaxAged { get; private set; }

        public IReactiveProperty<int> Aged { get; private set; }
        public IReactiveProperty<int> Price { get; private set; }
        public IReactiveProperty<bool> Rotten { get; private set; }
        public IObservable<Unit> OnDiscardAsObservable
        {
            get { return onDiscardSubjct; }
        }
        Subject<Unit> onDiscardSubjct = new Subject<Unit>();
    
        private readonly List<PriceTable> _priceTable;
        public IReadOnlyList<PriceTable> PriceTable
        {
            get { return _priceTable.AsReadOnly(); }
        }
        private int AgedToPrice(int aged)
        {
            foreach( var tbl in _priceTable )
            {
                if( tbl.min <= aged && aged <= tbl.max)
                {
                    return tbl.price;
                }
            }
            return 0;
        }

        private readonly List<CharactorTable> _charactorTable;
        public IReadOnlyList<CharactorTable> CharactorTable
        {
            get { return _charactorTable.AsReadOnly(); }
        }

        public void Aging(int days, EventCard eventCard)
        {
            int scale = 1;
            foreach( var ch in _charactorTable)
            {
                var val = eventCard.Weather[ch.type];
                if(val >= ch.threshold)
                {
                    scale *= ch.scale;
                }
            }
            Aged.Value += days * scale;
        }

        private readonly int MaxBet = 2;
        private List<Player> _betPlayersList;
        public void SetBetPlayer(Player player)
        {
            _betPlayersList.Add(player);
        }

        public void RemoveBetPlayer(Player player)
        {
            _betPlayersList.Remove(player);
        }

        // その肉に賭けられるかどうか
        public bool CanBet
        {
            get { return _betPlayersList.Count < MaxBet; }
        }

        public IReadOnlyList<string> NamesWhoBet
        {
            get
            {
                List<string> ret = new List<string>();
                foreach( var player in _betPlayersList)
                {
                    ret.Add(player.Name);
                }
                return ret.AsReadOnly();
            }
        }

        public FoodCard(int ID, string Name, string Description, List<PriceTable> priceTable, List<CharactorTable> charactorTable, int maxAged = 50)
        {
            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.MaxAged = maxAged;

            _priceTable = priceTable;
            _charactorTable = charactorTable;
            _betPlayersList = new List<Player>();

            Aged = new ReactiveProperty<int>(0);
            Price = Aged.Select(aged => AgedToPrice(aged)).ToReactiveProperty();
            Rotten = Aged.Select(aged => aged > this.MaxAged).ToReactiveProperty();
        }

        public void Reset()
        {
            Aged.Value = 0;
            Price.Value = 0;
            Rotten.Value = false;
            _betPlayersList = new List<Player>();
        }

        public void Discard()
        {
            onDiscardSubjct.OnNext(Unit.Default);
        }
    }
}
