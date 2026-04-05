using ECommerceApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly MongoDbService _mongoDbService;

        public HomeController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task<IActionResult> Index(string? category)
        {
            var items = await _mongoDbService.GetItemsAsync(category);
            ViewBag.SelectedCategory = category;
            ViewBag.Categories = new List<string>
            {
                "Vinyls",
                "Antique Furniture",
                "GPS Sport Watches",
                "Running Shoes",
                "Camping Tents"
            };
            return View(items);
        }
    }
}