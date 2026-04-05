using ECommerceApp.Models;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly MongoDbService _mongoDbService;

        public AdminController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            var items = await _mongoDbService.GetItemsAsync();
            var users = await _mongoDbService.GetUsersAsync();
            ViewBag.Users = users;
            return View(items);
        }

        // GET: /Admin/AddItem
        public IActionResult AddItem() => View();

        // POST: /Admin/AddItem
        [HttpPost]
        public async Task<IActionResult> AddItem(Item item)
        {
            await _mongoDbService.CreateItemAsync(item);
            return RedirectToAction("Index");
        }

        // POST: /Admin/DeleteItem
        [HttpPost]
        public async Task<IActionResult> DeleteItem(string id)
        {
            var item = await _mongoDbService.GetItemByIdAsync(id);
            if (item == null) return NotFound();

            // Remove ratings/reviews from affected users
            var users = await _mongoDbService.GetUsersAsync();
            foreach (var user in users)
            {
                var changed = false;

                var review = user.Reviews.FirstOrDefault(r => r.ItemId == id);
                if (review != null)
                {
                    user.Reviews.Remove(review);
                    changed = true;
                }

                if (changed)
                    await _mongoDbService.UpdateUserAsync(user.Id!, user);
            }

            await _mongoDbService.DeleteItemAsync(id);
            return RedirectToAction("Index");
        }

        // GET: /Admin/AddUser
        public IActionResult AddUser() => View();

        // POST: /Admin/AddUser
        [HttpPost]
        public async Task<IActionResult> AddUser(string username,
                                                   string password, string role)
        {
            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role
            };
            await _mongoDbService.CreateUserAsync(user);
            return RedirectToAction("Index");
        }

        // POST: /Admin/DeleteUser
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _mongoDbService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            // Remove this user's ratings and reviews from all items
            var items = await _mongoDbService.GetItemsAsync();
            foreach (var item in items)
            {
                var changed = false;

                var rating = item.Ratings.FirstOrDefault(r => r.UserId == id);
                if (rating != null)
                {
                    item.Ratings.Remove(rating);
                    item.AverageRating = item.Ratings.Any()
                        ? item.Ratings.Average(r => r.Score)
                        : 0;
                    changed = true;
                }

                var review = item.Reviews.FirstOrDefault(r => r.UserId == id);
                if (review != null)
                {
                    item.Reviews.Remove(review);
                    changed = true;
                }

                if (changed)
                    await _mongoDbService.UpdateItemAsync(item.Id!, item);
            }

            await _mongoDbService.DeleteUserAsync(id);
            return RedirectToAction("Index");
        }
    }
}
