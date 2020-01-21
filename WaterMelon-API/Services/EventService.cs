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

        public List<Event> Get() => _events.Find(events => true).ToList();

        public Event Get(string id) => _events.Find<Event>(_event => _event.Id == id).FirstOrDefault();

        public List<Event> GetFromName(string name) => _events.Find(_event => _event.Name.Contains(name)).ToList();

        public List<Event> GetFromUser(string id)
            => _events.Find(_event => _event.Guests.Contains(id)).ToList();
    }
}
