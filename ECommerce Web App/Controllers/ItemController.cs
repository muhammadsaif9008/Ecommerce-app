using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApp.Controllers
{
    public class ItemController : Controller
    {
        private readonly MongoDbService _mongoDbService;

        public ItemController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task<IActionResult> Details(string id)
        {
            var item = await _mongoDbService.GetItemByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Rate(string itemId, int score)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _mongoDbService.RateItemAsync(itemId, userId!, score);
            return RedirectToAction("Details", new { id = itemId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Review(string itemId, string text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            await _mongoDbService.ReviewItemAsync(itemId, userId!, username!, text);
            return RedirectToAction("Details", new { id = itemId });
        }
    }
}