using System.Collections.Generic;

namespace RuleManager
{
    class TimeIsDelicious
    {
        private List<FoodCard> _foodCardList;
        private List<FoodCard> _foodCardOnRound;

        private List<Player> _players;
        public List<Player> Players
        {
            get { return _players; }
        }

        public TimeIsDelicious ()
        {
            _foodCardList = new List<FoodCard>();
            for(int i=0; i<50; i++)
            {
                _foodCardList.Add(new FoodCard());
            }

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

        public void AdvanceTime(int time)
        {
            foreach(var card in _foodCardOnRound)
            {
                card.Aged += time;
            }
        }
    }
}
