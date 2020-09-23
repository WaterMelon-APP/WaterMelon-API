using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterMelon_API.Models
{
    public class publicUser
    {
        public String Id { get; set; }
        public String Username { get; set; }

        public String Email { get; set; }

        public String FirstName { get; set; } = "";

        public String LastName { get; set; } = "";

        public String Phone { get; set; } = "";

        public byte[] ProfilePicture { get; set; } = null;
    }
}
