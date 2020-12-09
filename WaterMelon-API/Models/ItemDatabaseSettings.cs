using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterMelon_API.Models
{
    public class ItemDatabaseSettings : IItemDatabaseSettings
    {
        public string ItemsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IItemDatabaseSettings
    {
        string ItemsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
