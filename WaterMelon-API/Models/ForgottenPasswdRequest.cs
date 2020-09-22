using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterMelon_API.Models
{
    public class ForgottenPasswdRequest
    {
        [FromQuery]
        public String Username { get; set; }

        [FromQuery]
        public String Email { get; set; }
    }
}
