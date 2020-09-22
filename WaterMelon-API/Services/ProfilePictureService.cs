using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using WaterMelon_API.Models;

namespace WaterMelon_API.Services 
{
    public class ProfilePictureService
    {
        private readonly IMongoCollection<ProfilePicture> _profilePictures;

        private readonly IConfiguration _configuration;
        public ProfilePictureService(IProfilePictureDatabaseSettings settings, IConfiguration config)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _configuration = config;
            _profilePictures = database.GetCollection<ProfilePicture>(settings.ProfilePicturesCollectionName);
        }

        public ProfilePicture Create(ProfilePicture pfp)
        {
            ProfilePicture pfpLoaded = _profilePictures.Find<ProfilePicture>(pfpQuery => pfpQuery.UserId.Equals(pfp.UserId)).FirstOrDefault();
            if (pfpLoaded == null)
            {
                _profilePictures.InsertOne(pfp);
                return pfp;
            } 
            return null;
        }

        public ProfilePicture GetFromUserId(string userId)
        {
            ProfilePicture pfpLoaded = _profilePictures.Find<ProfilePicture>(pfpQuery => pfpQuery.UserId.Equals(userId)).FirstOrDefault();
            if (pfpLoaded != null)
            {
                return pfpLoaded;
            }
            return null;
        }

        public ProfilePicture GetFromPictureId(string pictureId)
        {
            ProfilePicture pfpLoaded = _profilePictures.Find<ProfilePicture>(pfpQuery => pfpQuery.Id.Equals(pictureId)).FirstOrDefault();
            if (pfpLoaded != null)
            {
                return pfpLoaded;
            }
            return null;
        }

        public ProfilePicture RemoveFromPictureId(string pictureId)
        {
            ProfilePicture pfpLoaded = _profilePictures.Find(pfpQuery => pfpQuery.Id.Equals(pictureId)).FirstOrDefault();
            // if (pfpLoaded == null)
            // {
            //     return null;
            // }
            _profilePictures.DeleteOne(pfp => pfp.Id.Equals(pictureId));
            return pfpLoaded;
        }

        public ProfilePicture RemoveFromUserId(string userId)
        {
            ProfilePicture pfpLoaded = _profilePictures.Find(pfpQuery => pfpQuery.UserId.Equals(userId)).FirstOrDefault();
            if (pfpLoaded == null)
            {
                return null;
            }
            _profilePictures.DeleteOne(pfp => pfp.UserId.Equals(userId));
            return pfpLoaded;
        }
    }
}