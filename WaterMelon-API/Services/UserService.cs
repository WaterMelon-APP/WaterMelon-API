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

            if (user.Password != password)
                user = null;

            //create claims details based on the user information
            //var claims = new[] {
            //        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
            //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            //        new Claim("Id", user.Id.ToString()),
            //        new Claim("Name", user.Name),
            //        new Claim("UserName", user.Username),
            //        new Claim("Password", user.Password),
            //        new Claim("Email", user.Email)
            //       };

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            //var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

            //user.Token = new JwtSecurityTokenHandler().WriteToken(token);

            user.Token = "caca";

            return user;
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
