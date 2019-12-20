using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterMelon_API.Models
{
    public class Event
    {
        public String Name { get; set; }

        public DateTime Date { get; set; } 

        public DateTime UpdatedAtDate { get; set; }

        public String OwnerId { get; set; }

    }
}
