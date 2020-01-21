using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WaterMelon_API.Models
{
    public class Event
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        [JsonProperty("Name")]
        public String Name { get; set; }

        public String Owner { get; set; }

        public DateTime Date { get; set; }

        public String Address { get; set; }

        public String ItemListId { get; set; }

        public String[] Guests { get; set; }

        public Boolean Public { get; set; }
    }
}
