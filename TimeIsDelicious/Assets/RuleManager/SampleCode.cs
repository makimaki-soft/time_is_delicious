using System;

namespace RuleManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var timeIsDelicious = new TimeIsDelicious();

            foreach( var player in timeIsDelicious.Players )
            {
                Console.WriteLine(player.ID);
                player.PropertyChanged += (s, e) =>
                {
                    Player sender = s as Player;
                    if (e.PropertyName != "TotalEarned")
                    {
                        return;
                    }

                    Console.WriteLine(sender.ID + "さんの収入:" + sender.TotalEarned );
                };
            }

            var cards = timeIsDelicious.StartRound();
            foreach (var card in cards)
            {
                Console.WriteLine(card.MaxAged);
                card.PropertyChanged += (s, e) =>
                {
                    if( e.PropertyName == "Aged" )
                    {
                        return;
                    }

                    Console.WriteLine("=== " + e.PropertyName + " Changed(ID:" + card.GUID + ") ===");
                    Console.WriteLine("Aged : " + card.Aged);
                    Console.WriteLine("Price : " + card.Price);
                    Console.WriteLine("Rotten : " + card.Rotten);
                };
            }

            foreach (var player in timeIsDelicious.Players)
            {
                Console.WriteLine(player.ID + "さんの賭ける肉(0～5):");
                var read = Console.ReadLine();
                int idx;
                if( int.TryParse(read, out idx) )
                {
                    player.Bet(cards[idx]);
                }
            }

            Random rdm = new Random();
            while (true)
            {
                int dice = rdm.Next(1, 6);
                Console.WriteLine("サイコロの結果:" + dice);
               
                foreach (var player in timeIsDelicious.Players)
                {
                    Console.WriteLine(player.ID + "さんは売りますか？(y/n):");
                    var res = Console.ReadLine();
                    if(res=="y")
                    {
                        player.Sell(player.Bets[0]);
                    }
                }

                timeIsDelicious.AdvanceTime(dice);
            }

        }
    }
}
