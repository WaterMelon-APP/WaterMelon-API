using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterMelon_API.Models;
using WaterMelon_API.Services;

namespace WaterMelon_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet(Name = "GetAll")]
        [Authorize]
        public ActionResult<List<User>> Get() =>
           _userService.Get();
        
        // GET: api/Users/5
        [HttpGet("{id}", Name = "GetUser")]
        [Authorize]
        public ActionResult<User> Get(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost(Name = "login")]
        [Route("login")]
        public ActionResult<User> Login([FromBody] LoginRequest request)
        {
            User user = _userService.GetFromIds(request.Username, request.Password);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        // POST: api/Users
        [HttpPost(Name = "create")]
        [Route("create")]
        public ActionResult<User> Create(User user)
        {
            _userService.Create(user);

            return CreatedAtRoute("GetUser", new { id = user.Id.ToString() }, user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<User> Update(string id, User userIn)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            if (userIn.Password == string.Empty)
            {
                return BadRequest("Password must not be empty");
            }

            if (!string.IsNullOrEmpty(userIn.Email))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(userIn.Email);
                    if (addr.Address != userIn.Email)
                    {
                        return BadRequest("Invalid email address");
                    }
                }
                catch
                {
                    return BadRequest("Invalid email address");
                }
            }

            _userService.Update(id, userIn);

        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Remove(user.Id);

            return StatusCode(200);
        }
    }
}
