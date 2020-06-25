using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WaterMelon_API.Models;

namespace WaterMelon_API.Services
{
    public class ItemService
    {
        private readonly IMongoCollection<Item> _items;
        private readonly IConfiguration _configuration;

        public ItemService(IItemDatabaseSettings settings, IConfiguration config)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _configuration = config;
            _items = database.GetCollection<Item>(settings.ItemsCollectionName);
        }

        public Item Create(Item item)
        {
            /*
            Item itemLoaded = _items.Find<Item>(itemQuery => itemQuery.Name.Equals(item.Name) && itemQuery.FromEvent.Equals(item.FromEvent)).FirstOrDefault();
            if (itemLoaded == null)
            {
                _items.InsertOne(item);
                return item;
            }
            return null;*/
            _items.InsertOne(item);
            return item;
        }

        public List<Item> GetAllItems()
        {
            var result = _items.Find(items => true).ToList();
            return result;
        }

        public List<Item> GetItemsFromEvent(string id)
        {
            return _items.Find(_item => _item.FromEvent == id).ToList();
        }

        public Item GetFromItemId(string id)
        {
            var result = _items.Find<Item>(_item => _item.Id == id).FirstOrDefault();
            return result;
        }

        public Item GiveItem(string id, string user, int quantity)
        {
            Item newItem = _items.Find(_items => _items.Id == id).FirstOrDefault();
            var quantityDiff = 0;

            if (newItem.Bring != null && newItem.Bring.ContainsKey(user))
            {
                quantityDiff = quantity - newItem.Bring[user];
            }
            else
            {
                quantityDiff = quantity;
            }

            if (newItem.QuantityLeft >= quantityDiff)
            {
                newItem.QuantityLeft -= quantityDiff;
            }
            else
            {
                newItem.QuantityLeft = 0;
            }

            if (newItem.Bring == null)
            {
                newItem.Bring = new Dictionary<string, int>();
            }
            newItem.Bring[user] = quantity;

            _items.ReplaceOne(i => i.Id == id, newItem);
            return GetFromItemId(id);
        }

        public Item PayItem(string id, string user, int amount)
        {
            Item newItem = _items.Find(_items => _items.Id == id).FirstOrDefault();

            if(newItem.Pay != null && newItem.Pay.ContainsKey(user))
            {
                newItem.QuantityLeft = Convert.ToInt32(Math.Ceiling(Convert.ToDouble((newItem.QuantityLeft * newItem.Price + newItem.Pay[user]) / newItem.Price)));
            }
            newItem.QuantityLeft = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(newItem.QuantityLeft * newItem.Price - amount) / newItem.Price));
            
            if (newItem.Pay == null)
            {
                newItem.Pay = new Dictionary<string, int>();
            }
            newItem.Pay[user] =  amount;

            _items.ReplaceOne(i => i.Id == id, newItem);
            return GetFromItemId(id);
        }

        public Item UpdateItem(string id, ItemRequest itemRequest)
        {
            Item itemRecieved = new Item(itemRequest);
            _items.ReplaceOne(i => i.Id == id, itemRecieved);
            return GetFromItemId(id);
        }

        public void RemoveItemWithId(String itemId)
        {
            _items.DeleteOne(item => item.Id == itemId);
        }
    }
}
