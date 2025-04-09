using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ASP.netcore_Project.Models
{
    public class CartModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // Add this to track individual cart items

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } // ✅ Keep only this

        [BsonRepresentation(BsonType.String)]
        public string ProductId { get; set; }

        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public string Size { get; set; }
        public string SizeType { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        [BsonIgnore]
        public decimal TotalPrice => Quantity * Price;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
