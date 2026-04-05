using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECommerceApp.Models
{
    public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Seller { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;

        // Category-specific fields
        public int? BatteryLife { get; set; }      // GPS Watches only
        public int? Age { get; set; }               // Antiques & Vinyls only
        public string? Size { get; set; }           // Running Shoes only
        public string? Material { get; set; }       // Antiques & Running Shoes only

        // Ratings
        public double AverageRating { get; set; } = 0;
        public List<Rating> Ratings { get; set; } = new();

        // Reviews
        public List<Review> Reviews { get; set; } = new();
    }

    public class Rating
    {
        public string UserId { get; set; } = string.Empty;
        public int Score { get; set; }
    }

    public class Review
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}