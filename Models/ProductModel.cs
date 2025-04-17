using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ASP.netcore_Project.Models
{
    public class SizeModel
    {
        public string Size { get; set; }
        public int Quantity { get; set; }
    }

    public class ProductModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }  // e.g., "PROD-0001"

        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Gender { get; set; }  // "Men" or "Women"
        public string Brand { get; set; }
        public string Sku { get; set; }

        public string SizeType { get; set; } // "standard" or "waist"
        public List<SizeModel> Sizes { get; set; }

        public int StockQuantity { get; set; }

        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public int DiscountPercentage { get; set; }

        public List<string> ImageUrls { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsNewArrival { get; set; } = false;
    }
}
