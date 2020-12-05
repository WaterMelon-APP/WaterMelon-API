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
        [HttpGet("{id}")]
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
                return Unauthorized("Cet utilisateur a deja ete invite.");
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
            Invitation invitation = _invitationService.GetFromInvitationId(id);
            if (invitation == null)
            {
                return StatusCode(404, "Invitation non trouvee");
            }
            if (invitation.Status != 0)
            {
                return StatusCode(400, "Impossible d'accepter l'invitation car elle n'est pas en attente.");
            }
            var res = _invitationService.AcceptInvitation(id);
            var user = _userService.GetFromName(res.From);
            if (user == null)
            {
                return StatusCode(404, "Utilisateur non trouve.");
            }
            Notification notif = new Notification(res);
            notif.Type = "InvitationStatus";
            _notificationService.Create(notif);
            var result = _eventService.AddGuestToEvent(res.EventId, user.Username);
            _eventService.RemoveInvitationFromEvent(res);
            _invitationService.RemoveInvitationWithId(id);
            return StatusCode(200, "Invitation acceptee.");
        }

        [HttpPost]
        [Authorize]
        [Route("RefuseInvitation/{id}")]
        public ActionResult<Invitation> RefuseInvitation(string id)
        {
            Invitation invitation = _invitationService.GetFromInvitationId(id);
            if (invitation == null)
            {
                return StatusCode(404, "Invitation non trouvee");
            }
            if (invitation.Status != 0)
            {
                return StatusCode(400, "Impossible d'accepter l'invitation car elle n'est pas en attente.");
            }
            var res = _invitationService.RefuseInvitation(id);
            Notification notif = new Notification(res);
            notif.Type = "InvitationStatus";
            _notificationService.Create(notif);
            _eventService.RemoveInvitationFromEvent(res);
            _invitationService.RemoveInvitationWithId(id);
            return StatusCode(200, "Invitation refusee.");
        }

        [HttpGet("RetrieveInvitationsTo", Name = "RetrieveInvitationsTo")]
        [Authorize]
        [Route("RetrieveInvitationsTo/{to}")]
        public ActionResult<List<Invitation>> RetrieveInvitationsTo(string to)
        {
            var res = _invitationService.GetTo(to);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }

        [HttpGet("RetrieveInvitationsFrom", Name = "RetrieveInvitationsFrom")]
        [Authorize]
        [Route("RetrieveInvitationsFrom/{from}")]
        public ActionResult<List<Invitation>> RetrieveInvitationsFrom(string from)
        {
            var res = _invitationService.GetFrom(from);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }
    }
}
