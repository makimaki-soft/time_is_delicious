using System.Collections.Generic;

namespace RuleManager
{
    class TimeIsDelicious
    {
        private List<FoodCard> _foodCardList;
        private List<FoodCard> _foodCardOnRound;

        private List<EventCard> _eventCardList;
        private int _numEventCardOpend;

        private List<Player> _players;
        public List<Player> Players
        {
            get { return _players; }
        }

        public TimeIsDelicious ()
        {

            _foodCardList = TIDJsonReader.Parser.ReadFoodCards();
            _foodCardList.Shuffle();

            _eventCardList = TIDJsonReader.Parser.ReadEventCards();
            _eventCardList.Shuffle();
            _numEventCardOpend = 0;

            _players = new List<Player>();
            for (int i=0; i<4; i++)
            {
                _players.Add(new Player(i));
            }
        }

        public List<FoodCard> StartRound()
        {
            _foodCardOnRound = _foodCardList.Chunks(5);
            return _foodCardOnRound;
        }

        public void AdvanceTime(int time, EventCard eventCard)
        {
            foreach(var card in _foodCardOnRound)
            {
                card.Aging(time, eventCard);
            }
        }

        public EventCard OpenEventCard()
        {
            return _eventCardList[_numEventCardOpend++%_eventCardList.Count];
        }
    }
}
