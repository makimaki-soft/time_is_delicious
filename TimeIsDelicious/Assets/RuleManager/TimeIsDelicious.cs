using System.Collections.Generic;
using System.Linq;

namespace RuleManager
{
    public class TimeIsDelicious
    {
        static string[] Names = new string[4] { "鈴木精肉店", "マザーミート", "王丸農場" , "Chouette"};

        Deck<FoodCard> foodCardDeck;
        private List<FoodCard> _foodCardOnRound;

        public IReadOnlyList<FoodCard> RoundFoodCard
        {
            get { return _foodCardOnRound.AsReadOnly(); }
        }

        Deck<EventCard> eventCardDeck;

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
            foodCardDeck = new Deck<FoodCard>(TIDJsonReader.Parser.ReadFoodCards(), Deck<FoodCard>.Mode.AutoBack);
            eventCardDeck = new Deck<EventCard>(TIDJsonReader.Parser.ReadEventCards(), Deck<EventCard>.Mode.AutoBack);
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
            _foodCardOnRound = foodCardDeck.OpenTop(CardsPerRound).ToList();

            MakiMaki.Logger.Debug("肉カード山札 残り枚数:" + foodCardDeck.Remain);
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
            return eventCardDeck.OpenTop(1).First();
        }
    }
}
