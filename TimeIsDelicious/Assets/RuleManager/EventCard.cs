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

    public class EventCard
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

        public ReadOnlyDictionary<EventType,int> Weather { get; }

		public EventCard(int ID, string Name, string Decription, int temperature, int humidity, int wind)
        {
			_id = ID;
			_name = Name;
			_description = Decription;
            var tmp = new Dictionary<EventType, int>();
            tmp[EventType.Temperature] = temperature;
            tmp[EventType.Humid] = humidity;
            tmp[EventType.Wind] = wind;
            Weather = new ReadOnlyDictionary<EventType, int>(tmp);
        }
    }

}
