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
        public Event UpdateEvent(Event ev)
        {
            _events.ReplaceOne(e => e.Id == ev.Id, ev);
            return ev;
        }

        public Event AddItemToList(string eventId, string itemId)
        {
            Event modifiedEvent = GetFromEventId(eventId);

            if (modifiedEvent == null)
            {
                return null;
            }

            if (modifiedEvent.ItemList == null)
            {
                modifiedEvent.ItemList = new List<string>();
            }

            modifiedEvent.ItemList.Add(itemId);
            _events.ReplaceOne(e => e.Id == eventId, modifiedEvent);
            return GetFromEventId(eventId);
        }

        public Event RemoveItemFromList(string eventId, string itemId)
        {
            Event modifiedEvent = GetFromEventId(eventId);

            if (modifiedEvent == null)
            {
                return null;
            }

            modifiedEvent.ItemList.Remove(itemId);
            _events.ReplaceOne(e => e.Id == eventId, modifiedEvent);
            return GetFromEventId(eventId);
        }

        public void RemoveEventWithId(String id)
        {
            _events.DeleteOne(user => user.Id == id);
        }

        public Event RemoveGuestFromEvent(String id, EventGuestRequest eventGuestRequest)
        {
            Event eventLoaded = _events.Find(e => e.Id == id).FirstOrDefault();
            if (eventLoaded == null)
            {
                return null;
            }
            // remove
            var guestsList = eventLoaded.Guests.Where(s => s != eventGuestRequest.GuestName).ToList();
            eventLoaded.Guests = guestsList;
            _events.ReplaceOne(e => e.Id == id, eventLoaded);
            return GetFromEventId(id);
        }

        public Event AddGuestToEvent(String id, EventGuestRequest eventGuestRequest) 
        { 
            Event eventLoaded = _events.Find(e => e.Id == id).FirstOrDefault();
            if (eventLoaded == null)
            {
                return null;
            }
            var guestsList = eventLoaded.Guests;
            guestsList.Add(eventGuestRequest.GuestName);
            eventLoaded.Guests = guestsList;
            _events.ReplaceOne(e => e.Id == id, eventLoaded);
            return GetFromEventId(id); 
        }

        public Event AddGuestToEvent(String id, String guestUsername)
        {
            Event eventLoaded = _events.Find(e => e.Id == id).FirstOrDefault();
            if (eventLoaded == null)
            {
                return null;
            }
            var guestsList = eventLoaded.Guests;
            guestsList.Add(guestUsername);
            _events.ReplaceOne(e => e.Id == id, eventLoaded);
            return GetFromEventId(id);
        }

        public Event AddInvitationToEvent(Invitation invitation)
        {
            Event eventLoaded = _events.Find(e => e.Id == invitation.EventId).FirstOrDefault();
            if (eventLoaded == null)
            {
                return null;
            }
            var invitationsList = eventLoaded.InvitationList;
            invitationsList.Add(invitation.Id);
            _events.ReplaceOne(e => e.Id == eventLoaded.Id, eventLoaded);
            return GetFromEventId(eventLoaded.Id);
        }

        public Event RemoveInvitationFromEvent(Invitation invitation)
        {
            Event eventLoaded = _events.Find(e => e.Id == invitation.EventId).FirstOrDefault();
            if (eventLoaded == null)
            {
                return null;
            }
            var invitationsList = eventLoaded.InvitationList;
            invitationsList.Remove(invitation.Id);
            _events.ReplaceOne(e => e.Id == eventLoaded.Id, eventLoaded);
            return GetFromEventId(eventLoaded.Id);
        }
    }
}
