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

        [HttpGet("GetNotificationsFrom", Name = "GetNotificationsFrom")]
        [Authorize]
        [Route("GetNotificationsFrom/{from}")]
        public ActionResult<List<Notification>> GetNotificationsFrom(string from)
        {
            var res = _notificationService.GetFrom(from);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }

        [HttpGet("GetNotificationsTo", Name = "GetNotificationsTo")]
        [Authorize]
        [Route("GetNotificationsTo/{to}")]
        public ActionResult<List<Notification>> GetNotificationsTo(string to)
        {
            var res = _notificationService.GetTo(to);
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