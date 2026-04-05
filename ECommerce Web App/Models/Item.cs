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

        public int? BatteryLife { get; set; }
        public int? Age { get; set; }
        public string? Size { get; set; }
        public string? Material { get; set; }

        public double AverageRating { get; set; } = 0;
        public List<Rating> Ratings { get; set; } = new();
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