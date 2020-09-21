using System;
using Microsoft.AspNetCore.Mvc;

namespace WaterMelon_API.Models
{
    public class ProfilePictureRequest
    {
        [FromQuery]
        public String UserId { get;set; }
        [FromQuery]
        public String Filename { get;set; }
    }
}