using System;
using Microsoft.AspNetCore.Mvc;

namespace WaterMelon_API.Models
{
    public class EventGuestRequest
    {
        [FromQuery]
        public String GuestId { get; set; }
    }
}