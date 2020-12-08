using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterMelon_API.Models;
using WaterMelon_API.Services;
using WaterMelon_API;
using System.Threading.Tasks;

namespace WaterMelon_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly FacebookAuthService _facebookAuthService;
        private readonly GoogleAuthService _googleAuthService;
        private readonly EmailService _emailService;

        public UsersController(UserService userService, FacebookAuthService facebookAuthService, GoogleAuthService googleAuthService, EmailService emailService)
        {
            _userService = userService;
            _facebookAuthService = facebookAuthService;
            _googleAuthService = googleAuthService;
            _emailService = emailService;
        }

        [HttpGet(Name = "ping")]
        [Route("ping")]
        public IActionResult Ping()
        {
            return StatusCode(200, "Pong");
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

        [HttpGet("byName/{username}", Name = "byName")]
        [Route("users/byName")]
        [Authorize]
        public ActionResult<publicUser> byName(string username)
        {
            publicUser user = _userService.GetFromName(username);

            if (user == null)
            {
                return BadRequest("This user does not exist.");
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
                return BadRequest("Wrong combination username/password!");
            }
            return user;
        }

        // POST: api/Users
        [HttpPost(Name = "create")]
        [Route("create")]
        public ActionResult<User> Create(User user)
        {
            if (user.Email != "")
            {
                var validAddr = Regex.IsMatch(user.Email,
                            @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|" +
                            @"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*)" +
                            @"@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.)" +
                            @"{3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:" +
                            @"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$");
                if (!validAddr)
                {
                    return BadRequest("Invalid email address");
                }
            }

            string encryptedPassword = StringCipher.Encrypt(user.Password, "WaterMelonPasswd");

            user.Password = encryptedPassword;

            User createdUser = _userService.Create(user);
            if (createdUser == null) {
                return Unauthorized("User already exists.");
            }
            return CreatedAtRoute("GetUser", new { id = user.Id.ToString() }, user);
        }


        [HttpPost("recoverpasswd/{userId}",  Name = "recoverpasswd")]
        [Route("recoverpasswd")]
        public ActionResult<User> RecoverPasswd(string userId)
        {
            var user = _userService.Get(userId);

            if (user == null)
            {
                return StatusCode(401, "Aucun utilisateur trouvé avec cet identifiant.");
            }
            return _userService.Update(userId, user);
        }

        [HttpPost(Name = "forgotpasswd")]
        [Route("forgotpasswd")]
        public ActionResult<User> ForgotPasswd(ForgottenPasswdRequest fpr)
        {
            var user = _userService.CheckUsernameEmail(fpr.Username, fpr.Email);
            
            if (user == null)
            {
                return StatusCode(401, "Aucun utilisateur trouvé avec ces informations.");
            }
            this._emailService.Send(fpr.Email, _emailService.CreatePasswdRecoveryMailSubject(),
               _emailService.CreatePasswdRecoveryMailBody(user.Id));
            return user;
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
            } else if (userIn.Password != null)
            {
                userIn.Password = StringCipher.Encrypt(userIn.Password, "WaterMelonPasswd");
            }

            if (!string.IsNullOrEmpty(userIn.Email))
            {
                try
                {
                    var validAddr = Regex.IsMatch(userIn.Email,
                        @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"+
                        @"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*)"+
                        @"@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.)"+
                        @"{3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:"+
                        @"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$");
                    if (!validAddr)
                    {
                        return BadRequest("Invalid email address");
                    }
                }
                catch
                {
                    return BadRequest("Invalid email address");
                }
            }

            var props = typeof(User).GetProperties();

            foreach (var prop in props)
            {
                if (prop.GetValue(userIn) != null)
                {
                    prop.SetValue(user, prop.GetValue(userIn));
                }
            }

            return _userService.Update(id, user);

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

        // POST: api/Users/uploadpicture
        [HttpPost(Name = "uploadpicture")]
        [Route("uploadpicture")]
        public ActionResult<User> UploadPicture([FromBody] ProfilePictureRequest request)
        {
            try {
                byte[] binaryContent = System.IO.File.ReadAllBytes(request.Filename);
                if (binaryContent == null)
                {
                    return BadRequest("Error with file!");
                }
                var user = _userService.Get(request.UserId);
                if (user == null)
                {
                    return NotFound();
                }
                user.ProfilePicture = binaryContent;
                _userService.Update(request.UserId, user);
                return user;
            } catch (System.IO.FileNotFoundException e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("login-fb")]
        public async Task<IActionResult> LoginFacebook([FromBody] UserFacebookAuthRequest userFacebookRequest)
        {
            var authResponse = await _facebookAuthService.ValidateAccessTokenAsync(userFacebookRequest.AccessToken);
            if (!authResponse.Data.IsValid)
            {
                return StatusCode(500, new { error = "Error with token while logging with Facebook"});
            }
            var userInfo = await _facebookAuthService.GetUserInfoAsync(userFacebookRequest.AccessToken);
            var userLoaded = _userService.GetFromEmail(userInfo.Email);
            if (userLoaded == null)
            {
                var newUser = new User(userInfo.FirstName, userInfo.LastName, userInfo.Email);
                _userService.Create(newUser);
                newUser.Token = _userService.GenerateJwt();
                return Ok(newUser);
                
            }
            userLoaded.Token = _userService.GenerateJwt();
            return Ok(userLoaded);
        }

        [HttpPost]
        [Route("login-google")]
        public async Task<IActionResult> LoginGoogle([FromBody] UserGoogleAuthRequest userGoogleRequest)
        {
            var authResponse = await _googleAuthService.ValidateAccessTokenAsync(userGoogleRequest.AccessToken);
            if (authResponse.Email == null)
            {
                return StatusCode(400, "Error while logging with Google");
            }
            var userLoaded = _userService.GetFromEmail(authResponse.Email);
            if (userLoaded == null)
            {
                var newUser = new User(authResponse.Name, authResponse.GivenName, authResponse.Email);
                _userService.Create(newUser);
                newUser.Token = _userService.GenerateJwt();
                return Ok(newUser);
            }
            userLoaded.Token = _userService.GenerateJwt();
            return Ok(userLoaded);
        }
    }
}
