﻿using Microsoft.Extensions.Configuration;
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

        public User Get(String id) {
            User user = _users.Find<User>(user => user.Id == id).FirstOrDefault();
            return user.WithoutPassword();
        }

        public User GetFromIds(String username, String password)
        {
            User user = _users.Find<User>(user => user.Username.Equals(username)).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            if (user.Password == password)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._configuration["jwt:key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(this._configuration["jwt:issuer"],
                                             this._configuration["jwt:issuer"],
                                             expires: DateTime.Now.AddMinutes(30),
                                             signingCredentials: credentials);
                user.Token = new JwtSecurityTokenHandler().WriteToken(token);
                return user.WithoutPassword();
            }
            return null;
        }

        public User Create(User user)
        {
            User userLoaded = _users.Find<User>(userQuery => userQuery.Username.Equals(user.Username)).FirstOrDefault();
            if (userLoaded == null) {
                _users.InsertOne(user);
                return user;
            }
            return null;
        }

        public User Update(String id, User userIn)
        {
            _users.ReplaceOne(user => user.Id == id, userIn);
            return Get(id);
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
