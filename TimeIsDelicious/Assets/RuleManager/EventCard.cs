using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RuleManager
{
    public enum EventType
    {
        Temperature,
        Humid,
        Wind
    }

    public class EventCard : GameComponent
    {
        public ReadOnlyDictionary<EventType,int> Weather { get; }

        public EventCard(int temperature, int humidity, int wind)
        {
            var tmp = new Dictionary<EventType, int>();
            tmp[EventType.Temperature] = temperature;
            tmp[EventType.Humid] = humidity;
            tmp[EventType.Wind] = wind;
            Weather = new ReadOnlyDictionary<EventType, int>(tmp);
        }
    }

}
