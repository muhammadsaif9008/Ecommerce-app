using ECommerceApp.Models;
using MongoDB.Driver;

namespace ECommerceApp.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<Item> _items;
        private readonly IMongoCollection<User> _users;

        public MongoDbService(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDB:ConnectionString"];
            var databaseName = configuration["MongoDB:DatabaseName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            _items = database.GetCollection<Item>("Items");
            _users = database.GetCollection<User>("Users");
        }

        // ─── ITEM METHODS ───────────────────────────────────────────

        public async Task<List<Item>> GetItemsAsync(string? category = null)
        {
            if (!string.IsNullOrEmpty(category))
                return await _items.Find(i => i.Category == category).ToListAsync();

            return await _items.Find(_ => true).ToListAsync();
        }

        public async Task<Item?> GetItemByIdAsync(string id) =>
            await _items.Find(i => i.Id == id).FirstOrDefaultAsync();

        public async Task CreateItemAsync(Item item) =>
            await _items.InsertOneAsync(item);

        public async Task UpdateItemAsync(string id, Item item) =>
            await _items.ReplaceOneAsync(i => i.Id == id, item);

        public async Task DeleteItemAsync(string id) =>
            await _items.DeleteOneAsync(i => i.Id == id);

        // ─── USER METHODS ────────────────────────────────────────────

        public async Task<List<User>> GetUsersAsync() =>
            await _users.Find(_ => true).ToListAsync();

        public async Task<User?> GetUserByIdAsync(string id) =>
            await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

        public async Task<User?> GetUserByUsernameAsync(string username) =>
            await _users.Find(u => u.Username == username).FirstOrDefaultAsync();

        public async Task CreateUserAsync(User user) =>
            await _users.InsertOneAsync(user);

        public async Task UpdateUserAsync(string id, User user) =>
            await _users.ReplaceOneAsync(u => u.Id == id, user);

        public async Task DeleteUserAsync(string id) =>
            await _users.DeleteOneAsync(u => u.Id == id);

        // ─── RATING & REVIEW METHODS ─────────────────────────────────

        public async Task RateItemAsync(string itemId, string userId, int score)
        {
            var item = await GetItemByIdAsync(itemId);
            if (item == null) return;

            // Remove existing rating by this user if any
            item.Ratings.RemoveAll(r => r.UserId == userId);

            // Add new rating
            item.Ratings.Add(new Rating { UserId = userId, Score = score });

            // Recalculate average
            item.AverageRating = item.Ratings.Average(r => r.Score);

            await UpdateItemAsync(itemId, item);
        }

        public async Task ReviewItemAsync(string itemId, string userId, 
                                          string username, string text)
        {
            var item = await GetItemByIdAsync(itemId);
            if (item == null) return;

            var existing = item.Reviews.FirstOrDefault(r => r.UserId == userId);

            if (existing != null)
            {
                // Append edit as assignment requires
                existing.Text = existing.Text + " Edit: " + text;
            }
            else
            {
                item.Reviews.Add(new Review
                {
                    UserId = userId,
                    Username = username,
                    Text = text,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await UpdateItemAsync(itemId, item);

            // Also update user's review list
            var user = await GetUserByIdAsync(userId);
            if (user == null) return;

            var userReview = user.Reviews.FirstOrDefault(r => r.ItemId == itemId);
            if (userReview != null)
                userReview.Text = existing?.Text ?? text;
            else
                user.Reviews.Add(new UserReview
                {
                    ItemId = itemId,
                    ItemName = item.Name,
                    Text = text
                });

            // Recalculate user's average rating
            var allRatings = await _items.Find(_ => true).ToListAsync();
            var userRatings = allRatings
                .SelectMany(i => i.Ratings)
                .Where(r => r.UserId == userId)
                .ToList();

            user.AverageRating = userRatings.Any()
                ? userRatings.Average(r => r.Score)
                : 0;

            await UpdateUserAsync(userId, user);
        }
    }
}