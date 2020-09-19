using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace WaterMelon_API.Models
{
    public class ItemRequest
    {
        [FromQuery]
        public string Id { get; set; }

        [FromQuery]
        public string Name { get; set; }

        [FromQuery]
        public int Quantity { get; set; }

        [FromQuery]
        public int Price { get; set; }

        [FromQuery]
        public string About { get; set; }

        [FromQuery]
        public Dictionary<string, int> Bring { get; set; }

        [FromQuery]
        public Dictionary<string, int> Pay { get; set; }

        [FromQuery]
        public string FromEvent { get; set; }

        [FromQuery]
        public int QuantityLeft { get; set; }
    }
}
