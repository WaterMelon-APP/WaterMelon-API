using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WaterMelon_API.Models
{
    public class ProfilePicture
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get;set; }
        public string UserId { get;set; }
        public string ImageName { get; set; }
        public string ImageExtension { get; set; }
        public byte[] Content { get;set; }

        public ProfilePicture(string userId, string imageName, string imageExtension, byte[] content)
        {
            this.UserId = userId;
            this.ImageName = imageName;
            this.ImageExtension = imageExtension;
            this.Content = content;
        }
    }
}