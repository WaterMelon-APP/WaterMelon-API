namespace WaterMelon_API.Models
{
    public class ProfilePictureDatabaseSettings : IProfilePictureDatabaseSettings
    {
        public string ProfilePicturesCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IProfilePictureDatabaseSettings
    {
        string ProfilePicturesCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}