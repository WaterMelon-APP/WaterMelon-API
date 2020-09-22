using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterMelon_API.Models;
using WaterMelon_API.Services;

namespace WaterMelon_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {

        private readonly NotificationService _notificationService;
        private readonly EventService _eventService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/Notifications
        [HttpGet]
        [Authorize]
        public ActionResult<List<Notification>> Get() => _notificationService.Get();

         // GET: api/Notifications/5
        [HttpGet("{id}", Name = "GetNotification")]
        [Authorize]
        public ActionResult<Notification> GetFromId(string id)
        {
            var res = _notificationService.GetFromId(id);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }

        [HttpGet("{id}", Name = "GetForRecipient")]
        [Authorize]
        public ActionResult<List<Notification>> GetForRecipient(string id)
        {
            var res = _notificationService.GetForRecipient(id);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }

        [HttpGet("{id}", Name = "GetForSender")]
        [Authorize]
        public ActionResult<List<Notification>> GetForSender(string id)
        {
            var res = _notificationService.GetForSender(id);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }

        // POST: api/Notifications
        [HttpPost]
        [Authorize]
        public ActionResult<Notification> Post([FromBody] NotificationRequest notificationRequest)
        {
            Notification createdNotification = _notificationService.Create(new Notification(notificationRequest));
            return CreatedAtRoute("Get", new { id = createdNotification.Id }, createdNotification);
        }

        // PUT: api/Notifications/5
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<Notification> Put(string id, [FromBody] NotificationRequest notificationRequest)
        {
            var res = _notificationService.GetFromId(id);
            if (res == null)
            {
                return NotFound();
            }
            return _notificationService.Update(id, notificationRequest);
        }

        // DELETE: api/Notifications/5
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(string id)
        {
            var res = _notificationService.GetFromId(id);

            if (res == null)
            {
                return NotFound();
            }
            _notificationService.Remove(id);
            return StatusCode(200);
        }
    }
}