using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class PictureFormData
{
    [ModelBinder(Name = "userid")]
    public string UserId { get; set; }
    [ModelBinder(Name = "file")]
    public IFormFile File { get; set; }
}