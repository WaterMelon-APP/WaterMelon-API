using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WaterMelon_API.Models
{
     public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string About { get; set; }

        public Dictionary<string, int> Bring { get; set; }

        public Dictionary<string, decimal> Pay { get; set; }

        public string FromEvent { get; set; }

        public int QuantityLeft { get; set; }

        public Item(ItemRequest itemRequest)
        {
            this.Id = itemRequest.Id;
            this.Name = itemRequest.Name;
            this.Quantity = itemRequest.Quantity;
            this.Price = itemRequest.Price;
            this.About = itemRequest.About;
            this.Bring = itemRequest.Bring;
            this.Pay = itemRequest.Pay;
            this.FromEvent = itemRequest.FromEvent;
            this.QuantityLeft = itemRequest.QuantityLeft;
        }
    }
}
