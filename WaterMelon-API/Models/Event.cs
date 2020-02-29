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

        public String[] Guests { get; set; }

        public Boolean Public { get; set; }

        public String[] ItemList { get; set; }

        public Event(EventRequest eventRequest)
        {
            this.Id = eventRequest.Id;
            this.Name = eventRequest.Name;
            this.Owner = eventRequest.Owner;
            this.Date = eventRequest.Date;
            this.Address = eventRequest.Address;
            this.Guests = eventRequest.Guests;
            this.Public = eventRequest.Public;
            this.ItemList = eventRequest.ItemList;
        }
    }
}
