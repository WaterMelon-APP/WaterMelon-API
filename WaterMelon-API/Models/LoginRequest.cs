using System;
using Microsoft.AspNetCore.Mvc;

namespace WaterMelon_API.Models
{
    public class LoginRequest
    {
        [FromQuery]
        public String Username { get; set; }
        [FromQuery]
        public String Password { get; set; }
    }
}
