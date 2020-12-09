using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace WaterMelon_API.Models
{
    public class EventRequest
    {
        [FromQuery]
        public String Id { get; set; }

        [FromQuery]
        public String Name { get; set; }

        [FromQuery]
        public String Owner { get; set; }

        [FromQuery]
        public DateTime Date { get; set; }

        [FromQuery]
        public String Address { get; set; }

        [FromQuery]
        public List<string> Guests { get; set; }

        [FromQuery]
        public Boolean Public { get; set; }

        [FromQuery]
        public List<String> ItemList { get; set; }

        [FromQuery]
        public List<String> InvitationList { get; set; }
    }
}
