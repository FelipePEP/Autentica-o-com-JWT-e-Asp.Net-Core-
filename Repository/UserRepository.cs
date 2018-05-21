using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using aspNetCore.Business.Interfaces;
using aspNetCore.Business.Models;

namespace aspNetCore.Repository
{
    public class UserRepository  : IUser
    {
		private readonly IMongoDatabase _db;
		public readonly IMongoCollection<User> users;
		
		public UserRepository()
		{
			MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl("mongodb://localhost:27017"));
			var mongoClient = new MongoClient(settings);
			_db = mongoClient.GetDatabase("local");
			users = _db.GetCollection<User>("User");
		}	

		public IList<User> GetUsers()
		{
			var filter = Builders<User>.Filter.Exists(u => u.Id);
			var result = users.Find(filter).ToList();
			return result;
		}

		public User GetUser(string Login)
		{
			var filter = Builders<User>.Filter.Eq(u => u.Login, Login);
			var result = users.Find(filter).FirstOrDefault();
			return result;

		}

    }
}
