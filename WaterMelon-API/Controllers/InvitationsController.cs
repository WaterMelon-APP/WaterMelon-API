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
    public class InvitationsController : ControllerBase
    {

        private readonly InvitationService _invitationService;
        private readonly EventService _eventService;
        private readonly NotificationService _notificationService;
        private readonly UserService _userService;

        public InvitationsController(InvitationService invitationService)
        {
            _invitationService = invitationService;
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
            ev.InvitationList.Add(createdInvitation.Id);
            _eventService.UpdateEvent(ev);
            Notification notif = new Notification(createdInvitation);
            _notificationService.Create(notif);
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
            var user = _userService.GetFromName(res.To);
            if (user == null)
            {
                return NotFound();
            }
            Notification notif = _notificationService.Create(new Notification(res));
            var result = _eventService.AddGuestToEvent(res.EventId, user.Username);
            _eventService.RemoveInvitationFromEvent(res);
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
            return res;
        }

        [HttpGet]
        [Authorize]
        [Route("RetrieveInvitationsFromGuest/{guestName}")]
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
        [Route("RetrieveInvitationsFromGuest/{senderName}")]
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
