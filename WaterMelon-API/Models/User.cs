using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using Newtonsoft.Json;

namespace WaterMelon_API.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        [JsonProperty("Name")]
        public String Name { get; set; }

        public String Username { get; set; }

        public String Password { get; set; }

        public String Email { get; set; }
    }
}
