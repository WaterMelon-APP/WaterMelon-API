using System;
using Microsoft.AspNetCore.Mvc;

namespace WaterMelon_API.Models
{
    public class NotificationRequest
    {
        [FromQuery]
        public String DataId { get; set; }
        [FromQuery]
        public String Type { get; set; }
        [FromQuery]
        public String From { get; set; }
        [FromQuery]
        public String To { get; set; }
        [FromQuery]
        public String EventId { get; set; }
    }
}
