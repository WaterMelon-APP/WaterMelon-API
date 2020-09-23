using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterMelon_API.Models;
using WaterMelon_API.Services;

namespace WaterMelon_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationsController : ControllerBase
    {

        private readonly InvitationService _invitationService;
        private readonly EventService _eventService;
        private readonly NotificationService _notificationService;
        private readonly UserService _userService;

        public InvitationsController(InvitationService invitationService, EventService eventService, NotificationService notificationService,
            UserService userService)
        {
            _invitationService = invitationService;
            _eventService = eventService;
            _notificationService = notificationService;
            _userService = userService;
        }

        // GET: api/Invitations
        [HttpGet]
        [Authorize]
        public ActionResult<List<Invitation>> Get() => _invitationService.GetAllInvitations();

        // GET: api/Invitations/5
        [HttpGet("{id}", Name = "GetInvitation")]
        [Authorize]
        public ActionResult<Invitation> GetFromId(string id)
        {
            var res = _invitationService.GetFromInvitationId(id);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }

        // POST: api/Invitations
        [HttpPost]
        [Authorize]
        public ActionResult<Invitation> Post([FromBody] InvitationRequest invitationRequest)
        {
            Invitation createdInvitation = _invitationService.Create(new Invitation(invitationRequest));
            if (createdInvitation == null)
            {
                return Unauthorized("Invitation pending or accepted.");
            }
            Event ev = _eventService.GetFromEventId(invitationRequest.EventId);
            if (ev.InvitationList == null)
            {
                ev.InvitationList = new List<string>();
            }
            ev.InvitationList.Add(createdInvitation.Id);
            _eventService.UpdateEvent(ev);
            _notificationService.Create(new Notification(createdInvitation));
            return CreatedAtRoute("Get", new { id = createdInvitation.Id }, createdInvitation);
        }

        // PUT: api/Invitations/5
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<Invitation> Put(string id, [FromBody] InvitationRequest invitationRequest)
        {
            var res = _invitationService.GetFromInvitationId(id);
            if (res == null)
            {
                return NotFound();
            }
            return _invitationService.UpdateInvitation(id, invitationRequest);
        }

        // DELETE: api/Invitations/5
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(string id)
        {
            var res = _invitationService.GetFromInvitationId(id);

            if (res == null)
            {
                return NotFound();
            }
            _invitationService.RemoveInvitationWithId(id);
            return StatusCode(200);
        }

        [HttpPost]
        [Authorize]
        [Route("AcceptInvitation/{id}")]
        public ActionResult<Invitation> AcceptInvitation(string id)
        {
            var res = _invitationService.AcceptInvitation(id);
            if (res == null)
            {
                return NotFound();
            }
            var user = _userService.Get(res.To);
            if (user == null)
            {
                return NotFound();
            }
            Notification notif = _notificationService.Create(new Notification(res));
            var result = _eventService.AddGuestToEvent(res.EventId, user.Username);
            _eventService.RemoveInvitationFromEvent(res);
            _invitationService.RemoveInvitationWithId(id);
            return res;
        }

        [HttpPost]
        [Authorize]
        [Route("RefuseInvitation/{id}")]
        public ActionResult<Invitation> RefuseInvitation(string id)
        {
            var res = _invitationService.RefuseInvitation(id);
            if (res == null)
            {
                return NotFound();
            }
            Notification notif = _notificationService.Create(new Notification(res));
            _eventService.RemoveInvitationFromEvent(res);
            _invitationService.RemoveInvitationWithId(id);
            return res;
        }

        [HttpGet("RetrieveInvitationsFromGuest/{guestName}", Name = "RetrieveInvitationsFromGuest")]
        [Authorize]
        [Route("RetrieveInvitationsFromGuest/")]
        public ActionResult<List<Invitation>> RetrieveInvitationsFromGuest(string to)
        {
            var res = _invitationService.GetFromGuest(to);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }

        [HttpGet]
        [Authorize]
        [Route("RetrieveInvitationsFromSender/{senderName}")]
        public ActionResult<List<Invitation>> RetrieveInvitationsFromSender(string from)
        {
            var res = _invitationService.GetFromSender(from);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }
    }
}
