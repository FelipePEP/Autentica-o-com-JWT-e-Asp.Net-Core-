
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace aspNetCore.Business.Models
{
	public class User
	{
		[BsonId]
		[BsonIgnoreIfDefault]
		public ObjectId Id { get; set; }
		public string Login { get; set; }
		public string AccessKey { get; set; }
	}
}
