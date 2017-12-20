using System.Collections.Generic;

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

    public class FoodCard : GameComponent
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
                    if(_aged > _maxAged)
                    {
                        Rotten = true;
                        Price = 0;
                    }
                    else
                    {
                        Price = AgedToPrice(_aged);
                    }
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
            Aged += days * scale;
        }

        private readonly int MaxBet = 2;
        private List<Player> _betPlayersList;
        public void SetBetPlayer(Player player)
        {
            _betPlayersList.Add(player);
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

        public FoodCard(int ID, string Name, string Decription, List<PriceTable> priceTable, List<CharactorTable> charactorTable, int maxAged = 50)
        {
            _id = ID;
            _name = Name;
            _description = Decription;
            _aged = 0;
            _maxAged = maxAged;
            _price = 0;
            _rotten = false;
            _priceTable = priceTable;
            _charactorTable = charactorTable;
            _betPlayersList = new List<Player>();
        }
    }
}
