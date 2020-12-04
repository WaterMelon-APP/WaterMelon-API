using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterMelon_API.Models;
using WaterMelon_API.Services;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http.Headers;

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
        [Authorize]
        [Route("upload")]
        public async Task<IActionResult> Upload([FromForm] PictureFormData pictureFormData)
        {
            var filePath = Path.GetTempFileName();
            if (pictureFormData.File == null) {
                return NotFound("Veuillez envoyer un fichier.");
            }
            if (pictureFormData.File.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await pictureFormData.File.CopyToAsync(stream);
                    using (var memoryStream = new MemoryStream())
                    {
                        await pictureFormData.File.CopyToAsync(memoryStream);
                        if (memoryStream.Length < 4194304)
                        {
                            stream.CopyTo(memoryStream);
                            var mbytes = memoryStream.ToArray();
                        
                            if (mbytes == null)
                            {
                                return BadRequest("Error with file!");                                
                            }
                            ProfilePicture newPic = _profilePictureService.Create(new ProfilePicture (
                            pictureFormData.UserId,
                            ContentDispositionHeaderValue.Parse(pictureFormData.File.ContentDisposition).FileName.Trim('"'),
                            pictureFormData.File.ContentType,
                            mbytes
                        ));
                            if (newPic == null)
                            {
                                return BadRequest("Error while saving profile picture, empty file.");
                            }
                            return Ok(newPic);
                        }
                        else 
                        {
                            return BadRequest("File size must not exceed 4MB.");
                        }
                    }
                }
            }
            return BadRequest("Error creating picture");
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