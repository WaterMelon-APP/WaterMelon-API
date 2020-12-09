using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterMelon_API.Models
{
    public class EventDatabaseSettings : IEventDatabaseSettings
    {
        public string EventsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IEventDatabaseSettings
    {
        string EventsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
