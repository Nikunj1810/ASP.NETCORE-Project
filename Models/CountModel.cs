using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ASP.netcore_Project.Models
{
    public class CountModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = "dashboard-counts";

        public int TotalProducts { get; set; } = 0;
        public int TotalOrders { get; set; } = 0;
    }
}
