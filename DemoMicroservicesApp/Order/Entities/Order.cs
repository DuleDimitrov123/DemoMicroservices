using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Order.Entities;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public List<Product> Products { get; set; }

    public string Username { get; set; }

    public int UserServiceId { get; set; }
}
