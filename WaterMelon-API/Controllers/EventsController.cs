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

        public EventsController(EventService eventService)
        {
            _eventService = eventService;
        }

        // GET: api/Events
        [HttpGet]
        public ActionResult<List<Event>> Get() => _eventService.GetAllEvents();

        // GET: api/Events/5
        [HttpGet("{id}", Name = "Get")]
        [Authorize]
        public Event Get(int id)
        {
            Event event = _eventService.getFromEventId(id);
            if (event == null)
            {
                return NotFound();
            }
            return event;
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
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
