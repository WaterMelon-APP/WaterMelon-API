using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using WaterMelon_API.Models;
using WaterMelon_API.Helpers;

namespace WaterMelon_API.Services
{
    public class NotificationService
    {
        private readonly IMongoCollection<Notification> _notifications;
        private readonly IConfiguration _configuration;

        public NotificationService(IUserDatabaseSettings settings, IConfiguration config)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _configuration = config;
            _notifications = database.GetCollection<Notification>(settings.UsersCollectionName);
        }

        public List<Notification> Get() => _notifications.Find(notif => true).ToList();

        public Notification GetFromId(String id) {
            Notification notification = _notifications.Find<Notification>(notif => notif.Id == id).FirstOrDefault();
            return notification;
        }

        public List<Notification> GetFromEventId(String eventId) {
            var notifications = _notifications.Find<Notification>(notif => notif.EventId == eventId).ToList();
            return notifications;
        }

        public List<Notification> GetForRecipient(String userId) {
            var notifications = _notifications.Find<Notification>(notif => notif.To == userId).ToList();
            return notifications;
        }

        public List<Notification> GetForSender(String userId) {
            var notifications = _notifications.Find<Notification>(notif => notif.From == userId).ToList();
            return notifications;
        }

        public Notification Create(Notification notification)
        {
            Notification notificationLoaded = _notifications.Find<Notification>(notif => notif.Id == notification.Id).FirstOrDefault();
            if (notificationLoaded == null)
            {
                _notifications.InsertOne(notification);
                return notification;
            }
            return null;
        }

        public Notification Update(String id, NotificationRequest notificationRequest)
        {
            Notification notificationReceived = new Notification(id, notificationRequest);
            _notifications.ReplaceOne(i => i.Id == id, notificationReceived);
            return GetFromId(id);
        }

        public void Remove(String id)
        {
            _notifications.DeleteOne(notif => notif.Id ==id);
        }
    }
}
