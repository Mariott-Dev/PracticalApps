using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;
using System.Diagnostics;
using Packt.Shared; // NorthwindContext

namespace Northwind.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NorthwindContext db;
        //private readonly IHttpClientFactory clientFactory;

        public HomeController(ILogger<HomeController> logger, NorthwindContext injectedContext)    //, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            db = injectedContext;
            //clientFactory = httpClientFactory;
        }

        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Index()
        {
            _logger.LogError("This is a serious error (not really!)");
            _logger.LogWarning("This is your first warning!");
            _logger.LogWarning("Second warning!");
            _logger.LogInformation("I am in the Index method of the HomeController.");

            HomeIndexViewModel model = new
            (
                VisitorCount: (new Random()).Next(1, 1001),
                Categories: await db.Categories.ToListAsync(),
                Products: await db.Products.ToListAsync()
            );
            return View(model); // pass model to view
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ModelBinding()
        {
            return View(); // the page with a form to submit
        }
        [HttpPost]
        public IActionResult ModelBinding(Thing thing)
        {
            HomeModelBindingViewModel model = new(thing,
                !ModelState.IsValid,
                ModelState.Values
                    .SelectMany(state => state.Errors)
                    .Select(error => error.ErrorMessage)
            );
            return View(model);
        }

        // Matches /home/categorydetail/{id} by default so to
        // match /category/{id}, decorate with the following:
        [Route("category/{id}")]
        public async Task<IActionResult> CategoryDetail(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("You must pass a category ID in the route, for example, /Home/CategoryDetail/6");
            }

            Category? model = await db.Categories.Include(p => p.Products)
              .SingleOrDefaultAsync(p => p.CategoryId == id);

            if (model is null)
            {
                return NotFound($"CategoryId {id} not found.");
            }

            return View(model); // pass model to view and then return result
        }
    }
}
