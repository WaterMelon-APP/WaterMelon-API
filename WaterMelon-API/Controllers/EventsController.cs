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
