namespace RuleManager
{
    class FoodCard : GameComponent
    {
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
                        Price = _aged;
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

        public FoodCard()
        {
            _aged = 0;
            _maxAged = 50;
            _price = 0;
            _rotten = false;
        }
    }
}
