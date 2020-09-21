namespace WaterMelon_API.Models
{
    public class InvitationDatabaseSettings : IInvitationDatabaseSettings
    {
        public string InvitationsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IInvitationDatabaseSettings
    {
        string InvitationsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}