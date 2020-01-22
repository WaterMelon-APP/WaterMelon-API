using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WaterMelon_API.Models;

namespace WaterMelon_API.Services
{
    public class EventService
    {
        private readonly IMongoCollection<Event> _events;
        private readonly IConfiguration _configuration;

        public EventService(IEventDatabaseSettings settings, IConfiguration config)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _configuration = config;
            _events = database.GetCollection<Event>(settings.EventsCollectionName);
        }

        public Event Create(Event ev)
        {
            Event eventLoaded = _events.Find<Event>(eventQuery => eventQuery.Name.Equals(ev.Name) && eventQuery.Owner.Equals(ev.Owner)).FirstOrDefault();
            if (eventLoaded == null)
            {
                _events.InsertOne(ev);
                return ev;
            }
            return null;
        }

        public List<Event> GetAllEvents() 
        {
            var result = _events.Find(events => true).ToList();
            return result;
        } 

        public Event GetFromEventId(string id) {
            var result = _events.Find<Event>(_event => _event.Id == id).FirstOrDefault();
            return result;
        } 

        public List<Event> GetFromName(string name) => _events.Find(_event => _event.Name.Contains(name)).ToList();

        public List<Event> GetFromUser(string id)
            => _events.Find(_event => _event.Guests.Contains(id)).ToList();

        public Event UpdateEvent(string id, EventRequest eventRequest)
        {
            Event eventReceived = new Event(eventRequest);
            _events.ReplaceOne(e => e.Id == id, eventReceived);
            return GetFromEventId(id);
        }

        public void RemoveEventWithId(String id)
        {
            _events.DeleteOne(user => user.Id == id);
        }
    }
}
