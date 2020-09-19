using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace WaterMelon_API.Models
{
    public class DonationRequest
    {
        [FromQuery]
        public String userId { get; set; }

        [FromQuery]
        public int quantity { get; set; }

    }
}
