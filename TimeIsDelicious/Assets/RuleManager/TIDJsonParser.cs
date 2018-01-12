using System.Collections.Generic;
using UnityEngine;

namespace TIDJsonReader
{
    [System.Serializable]
    public class PriceTable
    {
        public int min;
        public int max;
        public int price;
    }

    [System.Serializable]
    public class CharactorTable
    {
        public string type;
        public int threshold;
        public int scale;
    }

    [System.Serializable]
    public class PriceTableTemptate
    {
        public int id;
        public PriceTable[] price_table;
    }

    [System.Serializable]
    public class FoodCard
    {
        public int id;
        public string name;
        public string description;
        public int rotten;
        public int price_tempate_id;
        public PriceTable[] price_table;
        public CharactorTable[] charactor_table;
    }

    [System.Serializable]
    public class Foods
    {
        public PriceTableTemptate[] price_template;
        public FoodCard[] food_cards;
    }

    [System.Serializable]
    public class Event
    {
		public int id;
		public string name;
		public string description;
        public int temperature;
        public int humidity;
        public int wind;
    }

    [System.Serializable]
    public class Events
    {
        public Event[] events;
    }

        public static class Parser
    {
        public static List<RuleManager.FoodCard> ReadFoodCards()
        {
            var cardSetting = Resources.Load("foodcards") as TextAsset;
            var inputItems = JsonUtility.FromJson<Foods>(cardSetting.text);

            var foodCardList = new List<RuleManager.FoodCard>();

            foreach (var foodCard in inputItems.food_cards)
            {
                var priceTableList = new List<RuleManager.PriceTable>();

                foreach (var template in inputItems.price_template)
                {
                    if (template.id == foodCard.price_tempate_id)
                    {
                        foreach (var table in template.price_table)
                        {
                            var rmPriceTable = new RuleManager.PriceTable();
                            rmPriceTable.min = table.min;
                            rmPriceTable.max = table.max;
                            rmPriceTable.price = table.price;
                            priceTableList.Add(rmPriceTable);
                        }
                        break;
                    }
                }

                var charactorList = new List<RuleManager.CharactorTable>();

                if (foodCard.charactor_table != null)
                {
                    foreach (var charactor in foodCard.charactor_table)
                    {
                        var rmCharactorTable = new RuleManager.CharactorTable();
                        if (charactor.type == "temperature")
                        {
                            rmCharactorTable.type = RuleManager.EventType.Temperature;
                        }
                        else if (charactor.type == "humid")
                        {
                            rmCharactorTable.type = RuleManager.EventType.Humid;
                        }
                        else if (charactor.type == "wind")
                        {
                            rmCharactorTable.type = RuleManager.EventType.Wind;
                        }
                        else
                        {
                            MakiMaki.Logger.Debug("Unknown charactor type");
                        }
                        rmCharactorTable.threshold = charactor.threshold;
                        rmCharactorTable.scale = charactor.scale;

                        charactorList.Add(rmCharactorTable);
                    }
                }
                foodCardList.Add(new RuleManager.FoodCard(foodCard.id, foodCard.name, foodCard.description, priceTableList, charactorList, foodCard.rotten));
            }

            return foodCardList;
        }

        public static List<RuleManager.EventCard> ReadEventCards()
        {
            var eventSetting = Resources.Load("eventcards") as TextAsset;
            var inputItems = JsonUtility.FromJson<Events>(eventSetting.text);

            var eventCardList = new List<RuleManager.EventCard>();

            foreach( var eventcard in inputItems.events)
            {
				eventCardList.Add(new RuleManager.EventCard(eventcard.id, eventcard.name, eventcard.description, eventcard.temperature, eventcard.humidity, eventcard.wind));
            }

            return eventCardList;
        }
    }
}