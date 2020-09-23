namespace WaterMelon_API.Models
{
    public class NotificationDatabaseSettings : INotificationDatabaseSettings
    {
        public string NotificationsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface INotificationDatabaseSettings
    {
        string NotificationsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}