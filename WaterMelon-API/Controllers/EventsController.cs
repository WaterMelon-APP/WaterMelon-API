using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterMelon_API.Models;
using WaterMelon_API.Services;

namespace WaterMelon_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly EventService _eventService;
        private readonly ItemService _itemService;
        private readonly NotificationService _notificationService;  

        public EventsController(EventService eventService, ItemService itemService, NotificationService notificationService)
        {
            _eventService = eventService;
            _itemService = itemService;
            _notificationService = notificationService;
        }

        // GET: api/Events
        [HttpGet]
        [Authorize]
        public ActionResult<List<Event>> Get() => _eventService.GetAllEvents();

        // GET: api/Events/5
        [HttpGet("{id}", Name = "Get")]
        [Authorize]
        public ActionResult<Event> GetFromId(string id)
        {
            var res = _eventService.GetFromEventId(id);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }

        [HttpGet]
        [Authorize]
        [Route("Search/{name}")]
        public ActionResult<List<Event>> Get(string name) => _eventService.GetFromName(name);

        [HttpGet]
        [Authorize]
        [Route("SearchFromUser/{id}")]
        public ActionResult<List<Event>> GetFromUser(string id) => _eventService.GetFromUser(id);

        // POST: api/Events
        [HttpPost]
        [Authorize]
        public ActionResult<Event> Post([FromBody] EventRequest eventRequest)
        {
            Event ev = new Event(eventRequest);
            Event createdEvent = _eventService.Create(ev);
            if (createdEvent == null)
            {
                return Unauthorized("Event already exists.");
            }
            return CreatedAtRoute("Get", new { id = ev.Id }, ev);
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<Event> Put(string id, [FromBody] EventRequest eventRequest)
        {
            var res = _eventService.GetFromEventId(id);
            if (res == null)
            {
                return NotFound();
            }
            return _eventService.UpdateEvent(id, eventRequest);
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(string id)
        {
            var res = _eventService.GetFromEventId(id);

            if (res == null)
            {
                return NotFound();
            }
            foreach (string item in res.ItemList) {
                _eventService.RemoveItemFromList(id, item);
                _itemService.RemoveItemWithId(item);
            }
            
            List<Notification> notifList = _notificationService.GetFromEventId(id);
            foreach (Notification notif in notifList)
            {
                _notificationService.Remove(notif.Id);
            }
            _eventService.RemoveEventWithId(id);
            return StatusCode(200);
        }

        [HttpPost]
        [Authorize]
        [Route("RemoveGuest/{id}")]
        public ActionResult<Event> RemoveGuest(string id, [FromBody] EventGuestRequest eventGuestRequest)
        {
            Event res = _eventService.RemoveGuestFromEvent(id, eventGuestRequest);
            if (res != null)
            {
                NotificationRequest request = new NotificationRequest();
                request.EventId = res.Id;
                request.From = eventGuestRequest.GuestName;
                request.To = res.Owner;
                request.Type = "QuitEvent";
                Notification notif = new Notification(request);
                notif.About = eventGuestRequest.GuestName + " a quitté votre événement \"" + res.Name + "\"";
                _notificationService.Create(notif);
            }
            return res;
        }

        [HttpPost]
        [Authorize]
        [Route("AddGuest/{id}")]
        public ActionResult<Event> AddGuest(string id, [FromBody] EventGuestRequest eventGuestRequest)
        {
            Event res = _eventService.AddGuestToEvent(id, eventGuestRequest);
            if (res != null)
            {
                NotificationRequest request = new NotificationRequest();
                request.EventId = res.Id;
                request.From = eventGuestRequest.GuestName;
                request.To = res.Owner;
                request.Type = "JoinedEvent";
                Notification notif = new Notification(request);
                notif.About = eventGuestRequest.GuestName + " a rejoint votre événement \"" + res.Name + "\"";
                _notificationService.Create(notif);
            }
            return res;
        }
    }
}
