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
            return res;
        }
    }
}
