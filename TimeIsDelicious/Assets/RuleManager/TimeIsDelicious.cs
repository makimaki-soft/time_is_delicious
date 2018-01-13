using System.Collections.Generic;
using System.Linq;

namespace RuleManager
{
    public class TimeIsDelicious
    {
        static string[] Names = new string[4] { "鈴木精肉店", "マザーミート", "王丸農場" , "Chouette"};

        private List<FoodCard> _foodCardList;
        private List<FoodCard> _foodCardOnRound;
        private List<FoodCard> _wasteCards;

        public IReadOnlyList<FoodCard> RoundFoodCard
        {
            get { return _foodCardOnRound.AsReadOnly(); }
        }

        private List<EventCard> _eventCardList;
        private int _numEventCardOpend;

        private List<Player> _players;
        public IReadOnlyList<Player> Players
        {
            get { return _players.AsReadOnly(); }
        }

        public int NumberOfPlayers
        {
            get { return Players.Count; }
        }

        public TimeIsDelicious ()
        {
            _foodCardOnRound = new List<FoodCard>();
            _wasteCards = new List<FoodCard>();
            _foodCardList = TIDJsonReader.Parser.ReadFoodCards();
            _foodCardList.Shuffle();

            _eventCardList = TIDJsonReader.Parser.ReadEventCards();
            _eventCardList.Shuffle();
            _numEventCardOpend = 0;

            _players = new List<Player>();
            
        }

        public IEnumerable<Player> CreatePlayers(int numOfPlayers)
        {
            for (int i = 0; i < numOfPlayers; i++)
            {
                _players.Add(new Player(i, Names[i]));
            }
            return _players;
        }

        private readonly int CardsPerRound = 5;
        public List<FoodCard> StartRound()
        {
            // 足りない場合は退避していた捨て札を
            if(_foodCardList.Count < CardsPerRound)
            {
                foreach(var waste in _wasteCards)
                {
                    waste.Reset();
                }

                _wasteCards.Shuffle();
                _foodCardList.AddRange(_wasteCards);
                _wasteCards.RemoveRange(0, _wasteCards.Count);
            }

            _foodCardOnRound = _foodCardList.Chunks(CardsPerRound);
            _foodCardList = _foodCardList.Skip(CardsPerRound).ToList();
            _wasteCards.AddRange(_foodCardOnRound); // 退避用

            MakiMaki.Logger.Debug("肉カード山札 残り枚数:" + _foodCardList.Count);
            return _foodCardOnRound;
        }

        public void AdvanceTime(int time, EventCard eventCard)
        {
            MakiMaki.Logger.Info("Aging(" + time.ToString() + ")");
            foreach(var card in _foodCardOnRound)
            {
                card.Aging(time, eventCard);
            }
        }

        public EventCard OpenEventCard()
        {
            // 山札を使い切ったらシャッフル
            if(_numEventCardOpend == _eventCardList.Count )
            {
                _eventCardList.Shuffle();
            }

            return _eventCardList[_numEventCardOpend++ % _eventCardList.Count];
        }
    }
}
