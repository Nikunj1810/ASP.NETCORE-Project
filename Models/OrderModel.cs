using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ASP.netcore_Project.Models
{
    public class OrderItemModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        public string ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string Size { get; set; }

        [Required]
        public string SizeType { get; set; }

        [Required]
        public int Quantity { get; set; }

        public string? ImageUrl { get; set; }
    }

    public class OrderModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public List<OrderItemModel> Items { get; set; }

        public ShippingInfo ShippingInfo { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        [Required]
        public double Subtotal { get; set; }

        [Required]
        public double DeliveryFee { get; set; }

        [Required]
        public double OrderTotal { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "pending";

        public DateTime LastUpdated { get; set; }
    }

    public class ShippingInfo
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string ZipCode { get; set; }
    }
}