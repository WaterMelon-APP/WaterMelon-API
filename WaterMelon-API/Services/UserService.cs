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
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IConfiguration _configuration;

        public UserService(IUserDatabaseSettings settings, IConfiguration config)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _configuration = config;
            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public List<User> Get() => _users.Find(user => true).ToList();

        public User Get(String id) =>
            _users.Find<User>(user => user.Id == id).FirstOrDefault();

        public User GetFromIds(String username, String password)
        {
            User user = _users.Find<User>(user => user.Username == username).FirstOrDefault();

            if (user.Password == password)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._configuration["jwt:key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(this._configuration["jwt:issuer"],
                                             this._configuration["jwt:issuer"],
                                             expires: DateTime.Now.AddMinutes(30),
                                             signingCredentials: credentials);
                user.Token = new JwtSecurityTokenHandler().WriteToken(token);
                return user;
            }
            return null;
        }

        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        public void Update(String id, User userIn)
        {
            _users.ReplaceOne(user => user.Id == id, userIn);
        }

        public void Remove(User userIn)
        {
            _users.DeleteOne(user => user.Id == userIn.Id);
        }

        public void Remove(String id)
        {
            _users.DeleteOne(user => user.Id == id);
        }
    }
}
