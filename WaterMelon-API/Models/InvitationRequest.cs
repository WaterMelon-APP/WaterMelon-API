using System;
using Microsoft.AspNetCore.Mvc;

namespace WaterMelon_API.Models
{
    public class InvitationRequest
    {
        [FromQuery]
        public String From { get; set; }

        [FromQuery]
        public String To { get; set; }

        [FromQuery]
        public String EventId { get; set; }

        [FromQuery] 
        public InvitationStatus Status { get; set; }
    }
}