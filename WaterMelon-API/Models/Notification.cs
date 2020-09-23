using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WaterMelon_API.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string DataId { get; set; }
        public string Type { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string EventId { get; set; }
        public string About { get; set; }

        public Notification(NotificationRequest notificationRequest)
        {
            this.DataId = notificationRequest.DataId;
            this.Type = notificationRequest.Type;
            this.From = notificationRequest.From;
            this.To = notificationRequest.To;
            this.EventId = notificationRequest.EventId;
            this.About = About;
        }

        public Notification(string id, NotificationRequest notificationRequest)
        {
            this.Id = id;
            this.DataId = notificationRequest.DataId;
            this.Type = notificationRequest.Type;
            this.From = notificationRequest.From;
            this.To = notificationRequest.To;
            this.EventId = notificationRequest.EventId;
            this.About = About;
        }

        public Notification(Invitation invitation)
        {
            this.DataId = invitation.Id;
            this.Type = "invitation";
            this.From = invitation.From;
            this.To = invitation.To;
            this.EventId = invitation.EventId;
            switch (invitation.Status)
            {
                case InvitationStatus.Accepted:
                {
                    this.About = "accepted";
                    break;
                }
                case InvitationStatus.Refused:
                {
                    this.About = "refused";
                    break;
                }
                default:
                {
                    this.About = "pending";
                    break;
                }
            }
        }
    }
}
