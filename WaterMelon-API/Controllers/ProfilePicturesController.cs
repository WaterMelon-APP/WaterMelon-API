using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterMelon_API.Models;
using WaterMelon_API.Services;

namespace WaterMelon_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilePicturesController : ControllerBase
    {
        private readonly ProfilePictureService _profilePictureService;

        public ProfilePicturesController(ProfilePictureService profilePictureService)
        {
            _profilePictureService = profilePictureService;
        }

        // GET: api/ProfilePictures/5
        [HttpGet("{id}", Name = "GetPicture")]
        [Authorize]
        [Route("GetPicture/{pictureId}")]
        public ActionResult<ProfilePicture> GetPicture(string pictureId)
        {
            var res = _profilePictureService.GetFromPictureId(pictureId);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }

         // GET: api/ProfilePictures/5
        [HttpGet("{id}", Name = "GetUserPicture")]
        [Authorize]
        [Route("GetUserPicture/{userId}")]
        public ActionResult<ProfilePicture> GetUserPicture(string userId)
        {
            var res = _profilePictureService.GetFromUserId(userId);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }

        [HttpPost(Name = "upload")]
        [Route("upload")]
        public ActionResult<ProfilePicture> Upload([FromBody] ProfilePictureRequest request)
        {
            byte[] binaryContent = System.IO.File.ReadAllBytes(request.Filename);
            if (binaryContent == null)
            {
                return BadRequest("Error with file!");
            }
            ProfilePicture newPic = _profilePictureService.Create(new ProfilePicture(request.UserId, binaryContent));
            if (newPic == null)
            {
                return BadRequest("Error with file!");
            }
            return newPic;
        }

        [HttpPost]
        [Authorize]
        [Route("RemoveUserPicture/{userId}")]
        public ActionResult<ProfilePicture> RemoveUserPicture(string userId)
        {
            var res = _profilePictureService.RemoveFromUserId(userId);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }

        [HttpPost]
        [Authorize]
        [Route("RemovePicture/{pictureId}")]
        public ActionResult<ProfilePicture> RemovePicture(string pictureId)
        {
            var res = _profilePictureService.RemoveFromPictureId(pictureId);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }
    }
}
