using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WaterMelon_API.Models
{
    public class Invitation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string SenderId { get; set; }

        public string GuestId { get; set; }

        public string EventId { get; set; }

        public InvitationStatus Status = 0;

        public Invitation(InvitationRequest request)
        {
            this.SenderId = request.SenderId;
            this.GuestId = request.GuestId;
            this.EventId = request.EventId;
        }
        public Invitation(string id, InvitationRequest request)
        {
            this.Id = id;
            this.SenderId = request.SenderId;
            this.GuestId = request.GuestId;
            this.Status = request.Status;
        }
    }

    public enum InvitationStatus 
    {
        Pending = 0,
        Accepted = 1,
        Refused = 2
    }
}