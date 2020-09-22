namespace WaterMelon_API.Models
{
    public class NotificationDatabaseSettings : INotificationDatabaseSettings
    {
        public string ItemsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface INotificationDatabaseSettings
    {
        string ItemsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}